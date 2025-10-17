namespace UVP.ExternalIntegration.Business.Services
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::UVP.ExternalIntegration.Business.Interfaces;
    using Newtonsoft.Json.Linq;
    using Scriban;
    using Scriban.Runtime;
    using Serilog;

    public class RenderingEngineService : IRenderingEngineService
    {
        private readonly ILogger _logger = Log.ForContext<RenderingEngineService>();

        public async Task<string> RenderPayloadAsync(string templateJson, object model)
        {
            try
            {
                _logger.Debug("Rendering payload template");
                if (templateJson is null)
                    throw new ArgumentNullException(nameof(templateJson));
                if (model is null)
                    throw new ArgumentNullException(nameof(model));

                // Build Scriban context
                var ctx = new TemplateContext
                {
                    MemberRenamer = member => member.Name,
                    StrictVariables = false
                };

                var globals = new ScriptObject();

                if (model is IDictionary<string, object> expando)
                {
                    bool isEarthMed = expando.ContainsKey("Doa") && expando.ContainsKey("User");
                    if (isEarthMed)
                    {
                        _logger.Information("Detected EARTHMED integration - applying field mappings");                        
                        var enrichedModel = ApplyEarthMedTransformations(expando);
                        foreach (var kvp in enrichedModel)
                        {
                            globals[kvp.Key] = ConvertDateTimesToStrings(kvp.Value);
                            _logger.Debug("Added {Key} to template globals", kvp.Key);
                        }
                    }
                    else
                    {
                        foreach (var kvp in expando)
                        {
                            globals[kvp.Key] = ConvertDateTimesToStrings(kvp.Value);
                            _logger.Debug("Added {Key} to template globals", kvp.Key);
                        }
                    }
                }
                else
                {
                    globals["Model"] = model;
                }

                ctx.PushGlobal(globals);

                var template = Template.Parse(templateJson);
                if (template.HasErrors)
                {
                    var errors = string.Join("; ", template.Messages.Select(m => m.ToString()));
                    throw new InvalidOperationException($"Template parse error(s): {errors}");
                }

                var rendered = template.Render(ctx);

                try
                {
                    JToken.Parse(rendered);
                }
                catch (Exception jsonEx)
                {
                    _logger.Error("Rendered output is not valid JSON: {Output}", rendered);
                    throw new InvalidOperationException("Template rendered invalid JSON", jsonEx);
                }

                _logger.Debug("Successfully rendered payload: {Rendered}", rendered);
                return await Task.FromResult(rendered);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error rendering payload");
                throw;
            }
        }
               
        /// <summary>
        /// Convert DateTime objects to strings recursively to prevent Scriban from reformatting dates
        /// </summary>
        private object ConvertDateTimesToStrings(object value)
        {
            if (value == null)
                return null;

            // Convert DateTime to string
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }

            // Convert nullable DateTime to string
            if (value.GetType() == typeof(DateTime?))
            {
                var nullableDateTime = (DateTime?)value;
                if (nullableDateTime.HasValue)
                {
                    return nullableDateTime.Value.ToString("yyyy-MM-dd");
                }
                return null;
            }

            // If it's a ScriptObject, recursively convert all DateTime values
            if (value is ScriptObject scriptObj)
            {
                var converted = new ScriptObject();
                foreach (var kvp in scriptObj)
                {
                    converted[kvp.Key] = ConvertDateTimesToStrings(kvp.Value);
                }
                return converted;
            }

            // If it's a dictionary (like ExpandoObject), recursively convert
            if (value is IDictionary<string, object> dict)
            {
                var converted = new ScriptObject();
                foreach (var kvp in dict)
                {
                    converted[kvp.Key] = ConvertDateTimesToStrings(kvp.Value);
                }
                return converted;
            }

            // If it's a list/array, convert each element
            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                var list = new List<object>();
                foreach (var item in enumerable)
                {
                    list.Add(ConvertDateTimesToStrings(item));
                }
                return list;
            }

            // Return other types as-is
            return value;
        }


        private object ConvertToScribanCompatible(object value)
        {
            if (value == null)
                return null;

            if (value is IDictionary<string, object> expando)
            {
                var scriptObj = new ScriptObject();
                foreach (var kvp in expando)
                {
                    scriptObj[kvp.Key] = ConvertToScribanCompatible(kvp.Value);
                }
                return scriptObj;
            }

            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                var list = new List<object>();
                foreach (var item in enumerable)
                {
                    list.Add(ConvertToScribanCompatible(item));
                }
                return list;
            }

            return value;
        }

        /// <summary>
        /// Apply EARTHMED-specific transformations to the model
        /// Creates a unified EarthMedRequest object with all mapped fields from multiple models
        /// </summary>
        private IDictionary<string, object> ApplyEarthMedTransformations(IDictionary<string, object> model)
        {
            var enriched = new Dictionary<string, object>(model, StringComparer.OrdinalIgnoreCase);

            try
            {
                // Extract all source models
                var doaModel = GetModelProperty<dynamic>(model, "Doa");
                var userModel = GetModelProperty<dynamic>(model, "User");
                var candidateModel = GetModelProperty<dynamic>(model, "Candidate");
                var doaCandidateModel = GetModelProperty<dynamic>(model, "DoaCandidate");
                var dutyStationModel = GetModelProperty<dynamic>(model, "DutyStationValue");
                var doaCandidateClearancesModel = GetModelProperty<dynamic>(model, "DoaCandidateClearances");

                // Extract IDs for computed fields
                long doaId = GetPropertyValue<long>(doaModel, "Id");
                long doaCandidateId = GetPropertyValue<long>(doaCandidateModel, "Id");
                long candidateId = GetPropertyValue<long>(candidateModel, "Id");

                // Get the UserId from Candidate to ensure we're using the correct User
                long userId = GetPropertyValue<long>(candidateModel, "UserId");
                _logger.Information("[EARTHMED] Candidate.UserId: {UserId}", userId);

                // Verify that the User model matches the Candidate's UserId
                long userModelId = GetPropertyValue<long>(userModel, "Id");
                if (userModelId != userId)
                {
                    _logger.Warning("[EARTHMED] User.Id ({UserModelId}) does not match Candidate.UserId ({UserId}). This may indicate incorrect model loading.",
                        userModelId, userId);
                }

                // Compute mandatory fields
                string referenceNumber = $"{doaId}_{doaCandidateId}";
                string sequenceNumber = "1";
                string clearanceType = "PE";

                // Extract dates
                DateTime? startDate = GetPropertyValue<DateTime?>(doaCandidateModel, "TentativeTravelDate") ?? DateTime.UtcNow;
                DateTime? endDate = GetPropertyValue<DateTime?>(doaCandidateModel, "ContractCalculatedEndDate") ?? DateTime.UtcNow;
                DateTime? requestDate = GetPropertyValue<DateTime?>(doaCandidateClearancesModel, "RequestedDate") ?? DateTime.UtcNow; // Current date as RequestDate
                DateTime? birthDate = GetPropertyValue<DateTime?>(userModel, "BirthDate");

                // Extract duty station info
                string dutyStationCode = GetPropertyValue<string>(dutyStationModel, "Code") ?? string.Empty;
                string dutyStationDescription = GetPropertyValue<string>(dutyStationModel, "ShortDescription") ?? string.Empty;

                // Extract candidate/user info
                string firstName = GetPropertyValue<string>(userModel, "FirstName") ?? string.Empty;
                string middleName = GetPropertyValue<string>(userModel, "MiddleName") ?? string.Empty;
                string lastName = GetPropertyValue<string>(userModel, "LastName") ?? string.Empty;
                string gender = GetPropertyValue<string>(userModel, "Gender") ?? string.Empty;
                string nationalityCode = GetPropertyValue<string>(userModel, "NationalityCode") ?? string.Empty;

                // FIXED: Email address from User model (associated with Candidate through Candidate.UserId = User.Id)
                // Priority: EmailAddress -> PersonalEmail from the User model that matches Candidate.UserId
                string emailAddress = GetPropertyValue<string>(userModel, "EmailAddress")
                    ?? GetPropertyValue<string>(userModel, "PersonalEmail")
                    ?? string.Empty;

                if (string.IsNullOrWhiteSpace(emailAddress))
                {
                    _logger.Warning("[EARTHMED] EmailAddress is empty for User.Id: {UserId} (Candidate.UserId: {CandidateUserId})",
                        userModelId, userId);
                }
                else
                {
                    _logger.Information("[EARTHMED] EmailAddress retrieved from User.Id: {UserId} -> {Email}",
                        userModelId, emailAddress);
                }

                // Extract candidate-specific info
                string indexNumber = GetPropertyValue<string>(candidateModel, "Id") ?? string.Empty;
                string employeeType = GetPropertyValue<string>(candidateModel, "EmployeeType") ?? "NVOL";
                string occupationGroup = GetPropertyValue<string>(candidateModel, "OccupationGroup") ?? string.Empty;
                string functionalTitleCode = GetPropertyValue<string>(candidateModel, "FunctionalTitleCode") ?? string.Empty;
                string functionalTitleDescription = GetPropertyValue<string>(candidateModel, "FunctionalTitleDescription")
                    ?? GetPropertyValue<string>(doaModel, "Name") ?? string.Empty;               

                // Extract DOA info
                string doaName = GetPropertyValue<string>(doaModel, "Name") ?? string.Empty;
                string organization = "UNV"; // Default organization

                _logger.Information("[EARTHMED] Mapping fields - ReferenceNumber: {RefNum}, Email: {Email}, DutyStation: {DutyStation}, IndexNumber: {IndexNum}",
                    referenceNumber, emailAddress, dutyStationCode, indexNumber);

                // Create the unified EarthMedRequest object
                dynamic earthMedRequest = new ExpandoObject();
                var requestDict = (IDictionary<string, object>)earthMedRequest;

                // Map all fields according to EARTHMED payload structure
                requestDict["ReferenceNumber"] = referenceNumber;
                requestDict["SequenceNumber"] = sequenceNumber;
                requestDict["ClearanceType"] = clearanceType;
                requestDict["RequestDate"] = requestDate;
                requestDict["RequestStatus"] = string.Empty;

                requestDict["IndexNumber"] = indexNumber;
                requestDict["FirstName"] = firstName;
                requestDict["MiddleName"] = middleName;
                requestDict["LastName"] = lastName;
                requestDict["DateOfBirth"] = birthDate;
                requestDict["Gender"] = gender;

                requestDict["EmailAddress"] = emailAddress;
                requestDict["NationalityCode"] = nationalityCode;

                requestDict["EmployeeType"] = employeeType;
                requestDict["OccupationGroup"] = occupationGroup;
                requestDict["Organization"] = organization;

                requestDict["FunctionalTitleCode"] = functionalTitleCode;
                requestDict["FunctionalTitleDescription"] = functionalTitleDescription;

                requestDict["DutyStationCode"] = dutyStationCode;
                requestDict["DutyStationDescription"] = dutyStationDescription;
                requestDict["DestinationDutyStationCode"] = dutyStationCode; // Same as DutyStationCode
                requestDict["DestinationDutyStationDescription"] = dutyStationDescription; // Same as DutyStationDescription

                requestDict["StartDate"] = FormatDate(startDate);
                requestDict["EndDate"] = FormatDate(endDate);

                // Add the EarthMedRequest object to the enriched model
                enriched["EarthMedRequest"] = earthMedRequest;

                // Also add computed fields at top level for template flexibility
                enriched["ReferenceNumber"] = referenceNumber;
                enriched["SequenceNumber"] = sequenceNumber;
                enriched["ClearanceType"] = clearanceType;
                enriched["DoaId_DoaCandidateId"] = referenceNumber; // For template compatibility

                // Create DutyStationValue alias for template compatibility
                dynamic dutyStationValue = new ExpandoObject();
                var dutyStationDict = (IDictionary<string, object>)dutyStationValue;
                dutyStationDict["Code"] = dutyStationCode;
                dutyStationDict["ShortDescription"] = dutyStationDescription;
                enriched["DutyStationValue"] = dutyStationValue;

                // Create property aliases with different casing for template compatibility
                // The template uses lowercase property names like birthDate, firstName, etc.
                CreateCandidateAliases(enriched, candidateModel, firstName, lastName, middleName, birthDate);
                CreateDoaAliases(enriched, doaModel, doaName);
                CreateUserAliases(enriched, userModel, emailAddress); // Pass emailAddress from User
                CreateDoaCandidateAliases(enriched, doaCandidateModel, emailAddress, startDate, endDate); // Add DoaCandidate aliases
                CreateDoaCandidateClearanceAliases(enriched, doaCandidateClearancesModel, requestDate);


                // Log mandatory field validation
                ValidateMandatoryFields(requestDict);

                _logger.Information("[EARTHMED] Transformation complete. Models available: {Models}",
                    string.Join(", ", enriched.Keys));

                return enriched;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[EARTHMED] Error applying transformations, returning original model");
                return model;
            }
        }

        /// <summary>
        /// Create Candidate model aliases with lowercase property names for template compatibility
        /// </summary>
        private void CreateCandidateAliases(Dictionary<string, object> enriched, dynamic candidateModel,
            string firstName, string lastName, string middleName, DateTime? birthDate)
        {
            if (candidateModel == null)
                return;

            dynamic candidateAlias = new ExpandoObject();
            var candidateDict = (IDictionary<string, object>)candidateAlias;

            // Copy all original properties from candidateModel
            var type = candidateModel.GetType();
            foreach (var prop in type.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(candidateModel);
                    if (value != null)
                    {
                        candidateDict[prop.Name] = value;
                        // Also add lowercase version
                        candidateDict[Char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1)] = value;
                    }
                }
                catch { }
            }

            // Add specific aliases that template expects
            //candidateDict["IndexNumber"] = indexNumber;
            candidateDict["firstName"] = firstName;
            candidateDict["lastName"] = lastName;
            candidateDict["middleName"] = middleName;
            candidateDict["birthDate"] = birthDate;

            enriched["Candidate"] = candidateAlias;
        }

        /// <summary>
        /// Create Doa model aliases with lowercase property names for template compatibility
        /// </summary>
        private void CreateDoaAliases(Dictionary<string, object> enriched, dynamic doaModel,
            string name)
        {
            if (doaModel == null)
                return;

            dynamic doaAlias = new ExpandoObject();
            var doaDict = (IDictionary<string, object>)doaAlias;

            // Copy all original properties from doaModel
            var type = doaModel.GetType();
            foreach (var prop in type.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(doaModel);
                    if (value != null)
                    {
                        doaDict[prop.Name] = value;
                        // Also add lowercase version
                        doaDict[Char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1)] = value;
                    }
                }
                catch { }
            }

            // Add specific aliases that template expects
            doaDict["name"] = name;

            enriched["Doa"] = doaAlias;
        }

        /// <summary>
        /// Create User model aliases with different property names for template compatibility
        /// </summary>
        private void CreateUserAliases(Dictionary<string, object> enriched, dynamic userModel, string emailAddress)
        {
            if (userModel == null)
                return;

            dynamic userAlias = new ExpandoObject();
            var userDict = (IDictionary<string, object>)userAlias;

            // Copy all original properties from userModel
            var type = userModel.GetType();
            foreach (var prop in type.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(userModel);
                    if (value != null)
                    {
                        userDict[prop.Name] = value;
                    }
                }
                catch { }
            }

            // Add PersonalEmail alias - using the email from User table (via Candidate.UserId relationship)
            userDict["PersonalEmail"] = emailAddress;
            userDict["EmailAddress"] = emailAddress;

            enriched["User"] = userAlias;
        }

        /// <summary>
        /// Create DoaCandidate model aliases to expose RequestorEmail
        /// </summary>
        private void CreateDoaCandidateAliases(Dictionary<string, object> enriched, dynamic doaCandidateModel, string emailAddress, DateTime? startDate, DateTime? endDate)
        {
            if (doaCandidateModel == null)
                return;

            dynamic doaCandidateAlias = new ExpandoObject();
            var doaCandidateDict = (IDictionary<string, object>)doaCandidateAlias;

            // Copy all original properties from doaCandidateModel
            var type = doaCandidateModel.GetType();
            foreach (var prop in type.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(doaCandidateModel);
                    if (value != null)
                    {
                        doaCandidateDict[prop.Name] = value;
                    }
                }
                catch { }
            }

            // Ensure RequestorEmail is available
            doaCandidateDict["RequestorEmail"] = emailAddress;
            doaCandidateDict["TentativeTravelDate"] = startDate;
            doaCandidateDict["ContractCalculatedEndDate"] = endDate;
            enriched["DoaCandidate"] = doaCandidateAlias;
        }

        private void CreateDoaCandidateClearanceAliases(Dictionary<string, object> enriched, dynamic doaCandidateClearanceModel, DateTime? requestDate)
        {
            if (doaCandidateClearanceModel == null)
                return;

            dynamic doaCandidateClearanceAlias = new ExpandoObject();
            var doaCandidateClearanceDict = (IDictionary<string, object>)doaCandidateClearanceAlias;

            // Copy all original properties from doaCandidateModel
            var type = doaCandidateClearanceModel.GetType();
            foreach (var prop in type.GetProperties())
            {
                try
                {
                    var value = prop.GetValue(doaCandidateClearanceModel);
                    if (value != null)
                    {
                        doaCandidateClearanceDict[prop.Name] = value;
                    }
                }
                catch { }
            }

            // Ensure RequestorEmail is available
            doaCandidateClearanceDict["RequestDate"] = requestDate;
            enriched["DoaCandidateClearances"] = doaCandidateClearanceAlias;
        }
        /// <summary>
        /// Get a model property from the dictionary
        /// </summary>
        private T? GetModelProperty<T>(IDictionary<string, object> model, string key) where T : class
        {
            if (model.TryGetValue(key, out var value) && value != null)
            {
                return value as T;
            }
            return null;
        }

        /// <summary>
        /// Extract a property value from a dynamic object using reflection
        /// </summary>
        private T? GetPropertyValue<T>(dynamic obj, string propertyName)
        {
            if (obj == null)
                return default(T);

            try
            {
                var type = obj.GetType();
                var prop = type.GetProperty(propertyName);
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    if (value == null)
                        return default(T);

                    // Handle type conversion
                    if (typeof(T) == typeof(string))
                    {
                        return (T)(object)value.ToString();
                    }
                    else if (value is T typedValue)
                    {
                        return typedValue;
                    }
                    else
                    {
                        // Try to convert
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "[EARTHMED] Failed to get property {PropertyName}", propertyName);
            }

            return default(T);
        }

        /// <summary>
        /// Format date to ISO 8601 string format (yyyy-MM-dd)
        /// </summary>
        private string FormatDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        /// <summary>
        /// Validate that all mandatory fields are present and not empty
        /// </summary>
        private void ValidateMandatoryFields(IDictionary<string, object> requestDict)
        {
            var mandatoryFields = new[]
            {
                "ReferenceNumber",
                "EmailAddress",
                "SequenceNumber",
                "ClearanceType",
                "RequestDate",
                "DutyStationCode",
                "DestinationDutyStationDescription",
                "StartDate"
            };

            var missingFields = new List<string>();

            foreach (var field in mandatoryFields)
            {
                if (!requestDict.TryGetValue(field, out var value) ||
                    value == null ||
                    (value is string strValue && string.IsNullOrWhiteSpace(strValue)))
                {
                    missingFields.Add(field);
                }
            }

            if (missingFields.Any())
            {
                _logger.Warning("[EARTHMED] Missing or empty mandatory fields: {Fields}", string.Join(", ", missingFields));
            }
            else
            {
                _logger.Information("[EARTHMED] All mandatory fields validated successfully");
            }
        }

        public async Task<string> RenderUrlAsync(string baseUrl, string pathTemplate, object model)
        {
            try
            {
                // Simple URL combination - path parameters are handled in IntegrationRunnerService
                var url = $"{baseUrl.TrimEnd('/')}/{pathTemplate.TrimStart('/')}";
                return await Task.FromResult(url);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error rendering URL");
                throw;
            }
        }

        public async Task<string> RenderPathParametersAsync(string pathTemplate, object model)
        {
            try
            {
                // This method can be used if we need to render path parameters with templates
                // Currently not used as we handle path parameters directly in IntegrationRunnerService
                if (string.IsNullOrEmpty(pathTemplate) || !pathTemplate.Contains("{{"))
                {
                    return await Task.FromResult(pathTemplate);
                }

                var ctx = new TemplateContext
                {
                    MemberRenamer = member => member.Name,
                    StrictVariables = false
                };

                var globals = new ScriptObject();

                if (model is IDictionary<string, object> expando)
                {
                    foreach (var kvp in expando)
                    {
                        globals[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    globals["Model"] = model;
                }

                ctx.PushGlobal(globals);

                var template = Template.Parse(pathTemplate);
                if (template.HasErrors)
                {
                    _logger.Warning("Path template has errors: {Errors}",
                        string.Join("; ", template.Messages));
                    return pathTemplate;
                }

                return await Task.FromResult(template.Render(ctx));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error rendering path parameters");
                return pathTemplate;
            }
        }
    }
}

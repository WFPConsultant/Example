FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy as base
USER root
WORKDIR /app
EXPOSE 80

FROM base AS final
#ironpdf dependencies
ENV DEBIAN_FRONTEND noninteractive
ENV ASPNETCORE_HTTP_PORTS=80
RUN sed -i 's|http://archive.ubuntu.com|http://security.ubuntu.com|g' /etc/apt/sources.list && \
	apt-get update && apt-get install -y --no-install-recommends \
	libasound2 \
	libatk-bridge2.0-0 \
	libc6 \
	libc6-dev \
	libdrm-common \
	libfontconfig1 \
	libgbm1 \
	libgdiplus \
	libgtk2.0-0 \
	libnss3 \
	libssl3 \
	libva-dev \
	libx11-xcb1 \
	libxcb-dri3-0 \
	libxkbcommon-x11-0 \
	libxrender1 \
	libxshmfence1 \
	openssl \
	wkhtmltopdf \
	xvfb \
	&& apt-get clean


WORKDIR /app
COPY ./publish/backend /app
ENTRYPOINT ["dotnet", "UVP.BackOffice.Api.Host.dll"]
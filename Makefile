bootstrapper := paket.bootstrapper.exe
bootstrapperpath := c:/Git/Paket/.paket/

all: clean test

updatepaket:
	@mkdir -p .paket \
	&& cp $(bootstrapperpath)/$(bootstrapper) .paket/$(bootstrapper) \
	&& .paket/$(bootstrapper) prerelease

install: updatepaket
	.paket/paket.exe install

build:
	dotnet restore --verbosity quiet
	C:\Program Files \(x86\)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe -nologo -v:q -property:GenerateFullPaths=true

#flags for good development experience with stack trace line number and filenames
test: build
	dotnet test --verbosity quiet --nologo

#clean out bin and obj
clean:
	@find . -type d \( -name "bin" -o -name "obj" \) | xargs rm -rf

.PHONY: install


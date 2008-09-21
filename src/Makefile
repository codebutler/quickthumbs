SOURCE_FILES= \
 QuickThumbsHandler.cs \
 template4dotnet.cs \
 ImageInfo.cs \
 Exif.cs \
 ExifUtils.cs \
 BitConverter.cs \
 MagickWandSharp.cs \
 Image.cs \
 Util.cs

REFERENCES=-lib:../web/bin -r:System.Web -r:System.Drawing -r:log4net.dll -r:Mono.Posix

RESOURCES=\
	-resource:resources/folder.png		\
	-resource:resources/error.png

all: QuickThumbs.dll 

QuickThumbs.dll: $(SOURCE_FILES)
	mcs -unsafe -debug -t:library -out:../web/bin/QuickThumbs.dll $(REFERENCES) $(RESOURCES) $(SOURCE_FILES)

QuickThumbsTool.exe: QuickThumbsTool.cs
	mcs -debug -r:../web/bin/QuickThumbs.dll -out:../web/bin/QuickThumbsTool.exe QuickThumbsTool.cs


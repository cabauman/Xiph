$InteropAssistantPath="D:\Ap\Inf\InteropSignatureToolkit"
$org=".\original"
$temp=".\temp"
md $temp
md "$temp\ogg"
md "$temp\vorbis"
copy "$org\libogg\include\ogg\*.h" "$temp\ogg"
copy "$org\libvorbis\include\vorbis\*.h" "$temp\vorbis"
copy ".\Convert\all.h" $temp
gc "$org\libogg\include\ogg\ogg.h"|foreach {$_ -replace "unsigned char[ ]*\*", "void *"}|sc "$temp\ogg\ogg.h"
&$InteropAssistantPath\sigimp /lang:cs /out:$temp\Xiph.cs "$temp\all.h" /includepath:"$temp,$temp\vorbis"
$cs=gc "$temp\Xiph.cs"
$cs=$cs|foreach {$_ -replace "System.Runtime.InteropServices.", ""}
$cs=$cs|foreach {$_ -replace 'System.', ''}
$cs=$cs|foreach {$_ -replace '\(\"<Unknown>\", EntryPoint=\"ogg', '("libogg.dll", EntryPoint="ogg'}
$cs=$cs|foreach {$_ -replace '\(\"<Unknown>\", EntryPoint=\"ov', '("libvorbisfile.dll", EntryPoint="ov'}
$cs=$cs|foreach {$_ -replace '\(\"<Unknown>\", EntryPoint=\"vorbis', '("libvorbis.dll", EntryPoint="vorbis'}
$cs=$cs|foreach {$_ -replace 'EntryPoint=', 'CallingConvention=CallingConvention.Cdecl, EntryPoint='}

#$cs=gc "Convert\header.cs"|$cs|cat "Convert\footer.cs"
gc ".\Convert\header.cs"|sc ".\LowLevel.cs"
$cs|ac ".\Xiph.cs"
gc ".\Convert\footer.cs"|ac ".\LowLevel.cs"

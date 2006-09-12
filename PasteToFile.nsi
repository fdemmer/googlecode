Name "PasteToFile"

OutFile "PasteToFile-0.2-setup.exe"

XPStyle on

Page directory
DirText "This will install the PasteToFile executeable, register a context menu extension and set up startmenu entries." \
  "Please specify the path where to install:"
InstallDir "$PROGRAMFILES"
Function .onVerifyInstDir
	IfFileExists $INSTDIR\p2f.exe +2
		Abort
FunctionEnd

Page custom StartMenuGroupSelect "" ": Start Menu Folder"
Function StartMenuGroupSelect
	Push $R1

	StartMenu::Select /checknoshortcuts "Don't create a start menu folder" /autoadd /lastused $R0 "PasteToFile"
	Pop $R1

	StrCmp $R1 "success" success
	StrCmp $R1 "cancel" done
		; error
		MessageBox MB_OK $R1
		Return
	success:
	Pop $R0

	done:
	Pop $R1
FunctionEnd

Page instfiles
Section
	# this part is only necessary if you used /checknoshortcuts
	StrCpy $R1 $R0 1
	StrCmp $R1 ">" skip

		CreateDirectory $SMPROGRAMS\$R0
		CreateShortCut "$SMPROGRAMS\$R0\Change options.lnk" $INSTDIR\p2f.exe --options

		SetShellVarContext All
		CreateDirectory $SMPROGRAMS\$R0
		CreateShortCut "$SMPROGRAMS\$R0\Change options.lnk" $INSTDIR\p2f.exe --options

		CreateDirectory $SMPROGRAMS\$R0
		CreateShortCut "$SMPROGRAMS\$R0\About.lnk" $INSTDIR\p2f.exe --about

		SetShellVarContext All
		CreateDirectory $SMPROGRAMS\$R0
		CreateShortCut "$SMPROGRAMS\$R0\About.lnk" $INSTDIR\p2f.exe --about

	skip:
SectionEnd



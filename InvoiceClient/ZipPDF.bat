"C:\Program Files\7-Zip\7z.exe" a -r -tzip %2\%1.zip %2\*.pdf
"C:\Program Files (x86)\GnuPG\bin\gpg.exe" --homedir C:\Project\Security\PGP --passphrase 70762419 --pinentry-mode loopback --default-key C9F6D772 --group google="DBA8C6C3 49942EF5 F94A0A32 32931EC7 0F333995 946833AC" -r google --batch --sign --yes --trust-model always -e "%2\%1.zip"
move "%2\%1.zip.gpg" %3
rem pause
rem del /q %2\*.*
rem rd %2
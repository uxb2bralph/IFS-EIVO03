echo on
F:
cd %2
"C:\Program Files (x86)\GnuPG\bin\gpg.exe" --homedir C:\Project\Security\PGP --passphrase 70762419 --pinentry-mode loopback --default-key C9F6D772 --group google="369AE9D7 86C4C1D5 946833AC" -r google --batch --sign --yes --trust-model always -o %4.zip.gpg -e %1.zip
move %1.gpg %3
del %1

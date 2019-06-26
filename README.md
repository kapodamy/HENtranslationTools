# PS3 HEN translation tools
translate the xml files is easy but if you expect change the `Welcome to PS3HEN` message in your locale here a couple of tools to help you. The welcome message is hardcoded inside of the file **PS3HEN.BIN**<br>
Actually tested with the **PS3 HEN v2.3.0** ¡Don't use in other versions can have diferent offsets in the embedded files!

**Warning**: a bad edition can lead to reinstall the PS3 HEN
<br>
<br>
Instructions:
1. Place a copy of your **PS3HEN.BIN** inside of the folder
2. Run `_unpack.bat` now you have 3 new files: stage0.bin stage2.bin HENplugin.sprx
3. Run `_unsign_plugin.bat` to decrypt the **HENplugin.sprx** file into **HENplugin.prx**

4. Now edit the strings inside of **HENplugin.prx** file with any hex editor or use the `hen_locate` utility

5. Run `_sign_plugin.bat` to encrypt the modified **HENplugin.prx** file (note: this will replace the HENplugin.sprx)
6. Finally run `_pack.bat` to update the **PS3HEN.BIN** file with your changes
7. ¡Done! copy the file back to the PS3

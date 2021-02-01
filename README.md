# Introduction
Tools to unpack NYAN-x-CAT's Lime Crypter, both tools are made by me & are CLI-based

# deLimer
This tool allows you to extract & decrypt the resource in the protected executable, however, the extracted resource isn't the original application, but is instead Stub.dll, which is basically the protector, you will need to use UnStub.exe to decrypt the resource contained in Stub.dll, which will give you the original, unprotected application

Usage: ```deLimer.exe <protected_executable>```

(Note: You will have to manually extract the resource out of Stub.dll)

# UnStub
This tool allows you to decrypt resources from the protected application's Stub.dll.

Usage: ```UnStub.exe <stub.dll_encrypted_resource> <protected_executable>```

# Weakness in Lime Crypter
- the AES Encryption password are the same for ```<protected_executable>``` & ```<protected_executable>```'s Stub.dll
- Encryption Salt isn't random

# Tip:
- You technically don't need deLimer if you manually extract the encrypted resource out of ```<protected_executable>```, you'll only need UnStub.exe

# Credits:
- 0xd4d (now wtfsck) - dnlib
- NYAN-x-CAT - Packer Maker

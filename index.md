***
>Addressable Manager & Setter => "Under Development && Test"
***
- [Install Package](#install-package)
  - [Install via Git URL](#install-via-git-url)
- [How to Use](#how-to-use)

***
#### Addressable Asset Setter
* Addressable Asset Setter From Folder a scriptable object will Assign labels addressable template and Auto Load Unload Mechanism and Load Unload on demand with ease via unity scriptable object container.

* Auto reorganize and structure selected assets into unity based folder, for example texture,audio,video ,mesh found within any selected folder and or recursive folders within and moves it to global Asset Folder AssetData for ease optional but recomended.

* Creates and updates a relevant  group in addressable groups as per folder assets contains and selected options.
***
#### Addressable Manager
* Addressable Manager takes scriptable objects generates and configured via setter and Easily instantiate & track asset via async operation. uses unitask instead of system.threading to make it compatible with WebGl.
* *** Need to do build etc as instructed in addressable system by unity 

***
### Install via Git URL

Open *Packages/manifest.json* with your favorite text editor. Add the following line to the dependencies block.

    {
        "dependencies": {
            "com.addressablesmanager.core": "https://github.com/SujanDuttaMishra/AddressablesManager.git"
        }
    }
  <p align="center">
  <img width="600" src="Images/packagemanager.png" alt="logo">
  </p>

    alternatively copy 
    https://github.com/SujanDuttaMishra/AddressablesManager.git
    and paste it in unity package manager 
***
## How to Use

[SetUp UnityAddressable](SetUpUnityAddressable.md)




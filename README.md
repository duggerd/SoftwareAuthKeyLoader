# SoftwareAuthKeyLoader

Software P25 Link Layer Authentication Key Loader (Windows .NET Console Application)

Supports all Manual Rekeying Features for Authentication per TIA-102.AACD-A

Download: [latest release](https://github.com/duggerd/SoftwareAuthKeyLoader/releases)

Disclaimer
----------

This utility is not intended for use in a production setting – security was not a consideration of the design. If you are operating a production system, use a commercial keyloader that supports P25 link layer authentication.

The subscriber identity and the link layer authentication key loaded into to the radio need to be entered into the trunking system in order for the authentication process to be successful. This tool by itself will not allow access to a link layer authentication protected trunking system.

Documentation
-------------

See `doc\sakl_manual.docx` (source) or `sakl_manual.pdf` (release)

Usage
-----

```
Usage: sakl.exe [OPTIONS]

Options:
  -h, -?, --help             show this message and exit
  -q, --quiet                do not show output
  -v, --verbose              show debug messages
  -i, --ip=VALUE             radio ip address [default 192.168.128.1]
  -p, --port=VALUE           radio udp port number [default 49165]
  -t, --timeout=VALUE        radio receive timeout (ms) [default 5000]
  -l, --load                 load key
  -z, --zeroize              zeroize key(s)
  -r, --read                 read key(s)
  -d, --device               device scope
  -a, --active               active scope
  -n, --named                named scope
  -w, --wacn=VALUE           wacn id (hex)
  -s, --system=VALUE         system id (hex)
  -u, --unit=VALUE           unit id (hex)
  -k, --key=VALUE            aes-128 encryption key (hex)

Examples:
  load key to the active suid
  /load /active /key 000102030405060708090a0b0c0d0e0f

  load key to the specified suid
  /load /named /wacn a4398 /system f10 /unit 99b584 /key 000102030405060708090a0b0c0d0e0f

  zeroize all keys
  /zeroize /device

  zeroize active key
  /zeroize /active

  zeroize specified key
  /zeroize /named /wacn a4398 /system f10 /unit 99b584

  read all keys
  /read /device

  read active key
  /read /active
```  

License
-------

SoftwareAuthKeyLoader is distributed under the MIT License.

Included open-source libraries:

 * Mono.Options: MIT License

TODO
----

* Unit tests
* Testing with SUs other than Motorola APX and ASTRO25 (XTS/XTL)

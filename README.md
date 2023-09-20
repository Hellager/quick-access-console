# QuickAccessConsole

QuickAccessConsole is a c# console application based on .net framework for handling windows quick access.

This is customized for [clean-recent](https://github.com/Hellager/clean-recent).

## Installation

### 1. From release page

Check [release page](https://github.com/Hellager/clean-recent/releases) and download `QuickAccessConsole.exe`.

### 2. Build it yourself

```c
$ git clone https://github.com/Hellager/quick-access-console
$ msbuild -t:restore QuickAccessConsole.sln
$ msbuild QuickAccessConsole.sln /p:platform="Any CPU" /p:configuration="Debug"
```

## Usage

```shell
$ .\QuickAccessConsole.exe

QuickAccessConsole 0.0.0.1
Copyright ©  2023 Steins.

ERROR(S):
  No verb selected.

  list       List current quick acess or supported languages.

  remove     Remove items from quick access.

  check      Check whether in quick access or show quick access or supported language.

  empty      Empty quick access.

  help       Display more information on a specific command.

  version    Display version information.
```



## Verbs & Options

### list

```shell
  -a, --all                     List both recent files and frequent folders.

  -r, --recent-files            List recent files.

  -f, --frequent-folders        List frequent folders.
  
  -u, --ui-culture              List system ui culture name.
```

### remove

```shell
  value pos. 0                  Targets to remove.
  
  -m, --menunames               Add unsported menu names
```

### check

```shell
  value pos. 0                    Targets to check.
  
  -q, --quick-access              Check whether in quick access.

  -s, --supported                 Check whether current system is supported

  -m, --menunames                 Add unsported menu names
```

### empty

```shell
  -a, --all                 Empty all quick access items.

  -r, --recent-files        Empty recent files.

  -f, --frequent-folders    Empty frequent folders.
```



## Contributing

Pull requests are welcome. 

It's better to make your own console applcation since this is customized.



## License

[Apache License 2.0](https://choosealicense.com/licenses/apache-2.0/)

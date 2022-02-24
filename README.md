# datetimefixer

A simple dotnet core CLI program to synchronize file date values (creation and modification dates) to the earliest of the two.  
Had to create it due to OneDrive messing up these dates and thus messing up the order of media in the onedrive app. 

## Instructions

Using this program is very simple, you just have to open the terminal (cmd, powershell, etc.) and call it with the path where your files are:

```bash
datetimefixer "/path/to/directory/"
```

說明:
1. 執行目錄中需含有初始化設定檔AutoUpdate.ini，檔案中指定更新的目錄位置如: ID=/autoupdate。

2. 步驟1指定的目錄位置下必須要有發布設定檔release.json，
(1)files為一至多個更新作業的項目、
(2)filename為欲更新的檔案名稱、
(3)version為判斷是否更新的切段時間點，若檔案大於此時間則進行更新、
(4)urlpath指定更新檔案的目標位置、
(5)smartmanpath指定SmartMan應用程式的資料夾位置。

例:

{
    "files": [
        {
            "filename": "Johnny.pdf",
            "version": "2024/11/13 12:00",
            "urlpath": "https://www.smartmancs.com.tw/download/Frank/Docs/Johnny.zip",
            "smartmanpath":"\\"
        }
    ]
}

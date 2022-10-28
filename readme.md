# 介紹

專案使用.net6 + vue3(vite)，透過 websocket 來交換資料。

目標是了解使用 json、 json compress(zlib)及 protobuf 之間傳輸資料的差異。

## 專案

本庫提供

1. SocketServer
2. socket-client-vue
3. proto

SocketServer 是後端專案，提供使用者 websocket 連線並回應
需使用支援.net6 的環境編譯運行。

socket-client-vue 是前端專案，
需使用 nodejs v14.18 以上的版本運行。

proto 是定義傳輸的格式資料夾。透過該定義好的傳輸格式，再轉換成各端使用的格式。

## 實作

將庫運行後，前端提供三種資料傳輸方式 json、 json compress(zlib)及 protobuf
可透過前端提供的介面切換請求並觀察資料處理方式

1. register 指令的 id 屬性，參數可以自行攜帶，後端會返回相同的請求資料
2. first 指令 會回復固定格式的後端資料

## 測試

運行裝置：
Processor Intel(R) Core(TM) i7-7700 CPU @ 3.60GHz 3.60 GHz
Installed RAM 16.0 GB

短資料：僅包含指令名稱
長資料：指令名稱 + 65535 長度資料(各語系)
原始資料結構：`{ action = "register", id="" }`

測試的項目為：

1. 短資料處理傳輸後的長度
2. 短資料 10000 筆處理花費時間
3. 長資料處理傳輸後的長度(5 語系)
4. 長資料 10000 筆處理花費時間(5 語系)

## 測試結果

由於運行裝置及資料來源隨機生成的關係，以下數據僅為約略參考：

### 短資料處理傳輸後的長度

json：41(byte)
json compress：37(byte)
protobuf：10(byte)

> 當傳輸的資料越短，protobuf 結構的資料會越有優勢(縮小 4 倍以上)

### 短資料 10000 筆處理花費時間

json：1657(ms)
json compress：2033(ms)
protobuf：10(ms)

> 使用 protobuf 處理交換資料幾乎不用花費什麼時間，這部分很有優勢

### 長資料處理傳輸後的長度(5 語系)

繁
json：65576(byte)
json compress：18650(byte)
protobuf：65549(byte)

簡
json：65576(byte)
json compress：18821(byte)
protobuf：65549(byte)

英
json：65576(byte)
json compress：18844(byte)
protobuf：65549(byte)

越
json：86467(byte)
json compress：26646(byte)
protobuf：86440(byte)

泰
json：193662(byte)
json compress：36275(byte)
protobuf：193623(byte)

> 非 ASCII(越，泰語系)由 UTF8 轉 byte array 長度會增加，
> 當傳輸資料內容越大時，protobuf 資料結構壓縮的優勢會變小；
> 而壓縮過後的 json，與未壓縮前傳輸資料小了約 3.2 倍(越)~5.3 倍(泰)

### 長資料 10000 筆處理花費時間(5 語系)

繁
json：2370(ms)
json compress：20742(ms)
protobuf：2725(ms)

簡
json：2363(ms)
json compress：21347(ms)
protobuf：2733(ms)

英
json：2420(ms)
json compress：21919(ms)
protobuf：2976(ms)

越
json：9602(ms)
json compress：40727(ms)
protobuf：2722(ms)

泰
json：8024(ms)
json compress：90383(ms)
protobuf：6304(ms)

> 長資料的處理上，在非 ascii 的語系上 protobuf 表現較好(越語系最佳)，繁、簡、英 語系則是 json 好一些些
> 而比較有無壓縮(不分 json 或 protobuf)所花費的時間，差異可以到 8 倍(繁) ~ 14 倍(越)

### 結論

就目前測試出來的資料看起來
protobuf 的資料結構在傳輸上相對於 json 格式來得有優勢(純指令小 4 倍，當參數越多優勢越明顯)
在傳輸的頻率極大且資料量不多的情況下，protobuf 處理資料跟結構大小都有很大的進步

json 資料在傳輸時是沒有壓縮的，透過封包解析的情況下很容易就可以看到資訊
protobuf 也是有相同情況，不過格式會稍微比較不可讀一些
json compress 的就得拿到資料解壓才看的到，如果有心且知道壓縮方式的話，還是能看的到

### 參考

[請求結果紀錄](https://docs.google.com/spreadsheets/d/1HQoRscKhR_UvttXspgSPrKg8S9mlLpif3awZb3I8iTk/edit#gid=1503123588)
[blog](https://fiddleliu0528.github.io/2022/10/26/使用protobuf格式透過websocket交換資料/#more)

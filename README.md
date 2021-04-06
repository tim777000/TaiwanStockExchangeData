# Taiwan Stock Exchange Data
> 余庭光

## 使用方式

* 連上網站後點擊TaiwanStockExchangeData，會顯示所有資料
* 點擊Create New，拉到最下方可以選擇檔案，將預期會用到的查詢資料匯入，例如：3月30日、3月31日證交所的資料
* 可一次匯入多個檔案
* 檔案匯入會檢查Database因此需要一些時間，請耐心等候
* 四月一日的交易資料已經完整匯入Database
* 匯入完成後即可進行查詢
    * 只輸入證券代號CodeName，From、To兩格為空：搜尋該代號所有資料，由新到舊排序
    * 只輸入特定日期From格，CodeName、To為空：搜尋該日期所有資料，以本益比高到低排序
    * 輸入證券代號加上From、To日期：搜尋該代號時間內殖利率為嚴格遞增的最長天數，以日期舊到新排序
* 每次查詢請先清空舊查詢
* 點擊Edit可以修改檔案
* 點擊Details會只顯示該筆資料
* 點擊Delete會刪除該資料

## 問題定義

問題一. 服務架設、資料呈現：使用者要使用該查詢功能，該用何種方法提供我們的服務
問題二. 資料匯入方式：要能動態匯入證交所的CSV
問題三. 搜尋方式：定義每個Search Form的責任

## 解題過程
由於之前沒什麼接觸過C#以及.NET Core，花了點時間參考文件以及範例，最後做出這個相當簡易的台灣證券交易所資料查詢Web App

* 讀完題目Spec後，在自己的電腦查詢了C#、.NET Core相關資料以及台灣證交所提供資料的方式後，在macOS的環境下，下載了Visual Studio for Mac Version 8.9.3(build 13)，以此作為開發環境
* 接著瀏覽了一些微軟關於.NET的文件，大概了解到ASP.NET比較適合Web相關開發，照著Web Api的教程走了一下發現時常會出現一些資料顯示的狀況，於是決定改從Web App下手
* 按照連結的教程 https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/razor-pages/?view=aspnetcore-5.0 簡單做出了一個可以瀏覽電影的頁面，該頁面能以關鍵字查詢相關電影，也可以動態增加、刪除或是編輯電影資料，與題目Spec有些類似處，因此決定根據此範例實作Take Home Engineering Challenge（問題一解決：使用Web App提供服務）
* 證券的英文為Security，我們所要加入、查找的資料都是證券，因此要先建立證券的Model如下：

Security.cs
```csharp=
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaiwanStockExchangeData.Models
{
    public class Security
    {
        public long ID { get; set; }
        public string CodeName { get; set; }
        public string Name { get; set; }
        public double DividendYield { get; set; }
        public long DividendYear { get; set; }
        public double PriceToEarningRatio { get; set; }
        public double PriceToBookRatio { get; set; }
        public string FinancialStatements { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
```
* 各個欄位代表意思：
    * ID是在證券資料存入Database時必須的欄位，會被設為Primary Key
    * CodeName：證券代號
    * Name：證券名稱
    * DividendYeild：殖利率
    * DividendYear：股利年度
    * PriceToEarningRatio：本益比
    * PriceToBookRatio：股價淨值比
    * FinancialStatements：財報年/季
    * Date：日期(月/日/年)
* Visual Studio能讓Razor Page使用Entity Framework，能在資料庫的使用上較為方便，選擇CRUD的Framework，此時會建立Create、Delete、Details、Edit、Index頁面，以及日後使用資料庫時會用到的DataContext
* 接著從code中我們可以發現，教程中的Razor Page並沒有使用到像是MVC之類的架構，僅僅單純是一個前端的頁面，以及負責該頁面後端的Model而已，這在實際上似乎不是一個太好的架構，會讓前後端太緊密，由於還不是太熟C#，對後端的知識也不是很充足，因此選擇不改架構繼續做
* 閱讀了這幾個頁面的程式碼後，我們了解到幾乎所有動作都是由一個OnPostAsync觸發（Details除外），而此項作業中我們最主要要做到的Create以及Index（搜尋、顯示等）部分都與OnPostAsync相關
* 有了Create頁面後我們已經能手動加入資料了，但證交所單一一個CSV檔案可能就有好幾千筆Data，題目的Spec中也有提到要能動態加入資料，因此我們需要找方法能匯入整份CSV檔：

Create.cshtml
```htmlembedded=
<form method="post" enctype="multipart/form-data">
    <div class="form-group">
        <input type="file" name="files" multiple accept="text"/>
    </div>
    <div class="form-group">
        <input type="submit" value="Upload" class="btn btn-primary" />
    </div>
</form>
```
* 在Create.cshtml中建立一個額外接收檔案的From，且該From能接收多個檔案

Create.cshtml.cs
```csharp=
public async Task<IActionResult> OnPostAsync(List<IFormFile> files)
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    foreach (var file in files)
    {
        if (file.Length > 0)
        {
            var path = $@"CSV/{file.FileName}";
            var fileName = file.FileName.Split('_');
            fileName = fileName[3].Split('.');
            var result = string.Empty;
            using (var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding("Big5"), true))
            {
                while ((result = reader.ReadLine()) != null)
                {
                    ~~~
                }
            }
        }
    }
}
```
* 當該From被submit後，Create.cshtml.cs後端會透過OnPostAsync接收到，同時被上傳的檔案儲存於List<IFormFile> files中，通過foreach將各個CSV檔案抓出來，再用StreamReader讀出內容後進行解析，將讀入的資料存入Database，同時將上傳檔案存入CSV資料夾，細部邏輯不贅述（問題二解決）
* 最後是搜尋問題，教程於Index.cshtml中加入了能夠根據片名搜尋電影的功能，模仿此功能，我們將片名換成證券代號，同時Form中再加上開始以及結束日期：

Index.cshtml
```htmlembedded=
<form>
    <p>
        CodeName: <input type="text" asp-for="SearchString" />
        From: <input type="text" asp-for="StartDate" />
        To: <input type="text" asp-for="EndDate" />
        <input type="submit" value="Search" />
    </p>
</form>
```
* 前端submit Form後，搜尋時便可以利用這些資料進行判斷

Index.cshtml.cs

```csharp=
public async Task OnGetAsync()
{
    var securities = from s in _context.Security select s;

    if(!string.IsNullOrEmpty(SearchString) && !StartDate.HasValue && !EndDate.HasValue)
    {
        securities = securities.Where(s => s.CodeName.Contains(SearchString));
        securities = securities.OrderByDescending(s => s.Date);
    }

    if(string.IsNullOrEmpty(SearchString) && StartDate.HasValue && !EndDate.HasValue)
    {
        securities = securities.Where(s => s.Date == StartDate);
        securities = securities.OrderByDescending(s => s.PriceToEarningRatio);
    }
~~~
}
```

* Index.cshtml.cs後端接收到Form後，依照“使用方式”提及的三種格式：
    * 只有CodeName
    * 只有From Date
    * CodeName、From Date、End Date三者皆有

對Database進行Query，再將得出的答案呈現到Index頁面（問題三解決）

## 額外討論
* 後端在讀取CSV檔時，由於證交所CSV似乎是使用Big5 Encode，導致一開始不斷出現亂碼，而又.NET Core Default似乎沒支援Big5編碼，因此必須額外安裝套件
* 第三種搜尋：輸入證券代號加上From、To日期：搜尋該代號時間內殖利率為嚴格遞增的最長天數，以日期舊到新排序
我採用Brute-Force的方式，尋找嚴格遞增數列，再確認該數列是否為最常嚴格遞增數列，優化上還有可進步的地方
* 整體Web App架構，由於時間以及知識上的不足，各種程式功能切割並不完善，目前的Extensibility還不佳
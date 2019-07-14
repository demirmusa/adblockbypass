# **AdBlockBypass**
Simple way to bypass adblock like browser extensions

### **What does this library do ?**

Adblock extensions use marks to block html elements of applications.(something like class name, id, tag name, etc.) This library prevents the adblocking by changing these markers each time cache expires. In this way, your ads markers will be unpredictable.

### **Usage**

***Step 1*** 

Reference  `ABB` to your project.

***Step 2***

Define your ad pages and their js and css files in `Startup.cs`  (IServiceCollection)

```csharp
services.AddAdBlockBypass(opt =>
{
    opt.CacheExpireTimeSec = 60*2;//it will cache all keys for 2 min
    opt.DefaultFilePath = Directory.GetCurrentDirectory() + "/wwwroot/";//your ads css files and js files default path
    //----define all css files and js files that your ads will need
    opt.AddCSSFiles(
        new ABBFile()
        {
            FileName = "adblockme.css",//file name
            KeysToReplace = new List<string>() { "adBlockTest", "adBlockTest2" }//marks that your file use (like '.adBlockTest', '#adBlockTest2' no need to add . or # prefix)
        },
        new ABBFile()
        {
            FileName = "myadd.css",
            FilePath = Directory.GetCurrentDirectory() + "/wwwroot/myfolder",//this is where adblockbypass looks for myadd.css(it will not use DefaultFilePath)// if your file in another path you can define it
            KeysToReplace = new List<string>() { "adBlockTest" }
        }
    );
    opt.AddJSFiles(
        new ABBFile()
        {
            FileName = "adblockme.js",
            KeysToReplace = new List<string>() { "adBlockTest" }
        }
    );
    //----
    //after you define css and js files you can define your ad pages
    
    // define what you want. which requests will you bypass
    opt.AddPages(
        new ABBPage("/myadd")// work for '/myadd' uri absolute path
        .AddJsFiles("adblockme.js") //js files that you need to bypass in this request  (you have to define your file in 'opt.AddJSFiles')
        .AddCssFiles("adblockme.css", "myadd.css")//css files that you need to bypass in this request (you have to define your file in 'opt.AddCSSFiles')
        .AddAnotherKeysToBypass("myTestClass", "myTestId", "myTestClass2"),//another keys to bypass. (adblock can block your html element with any unique parent of it.If your ad has parent element which adblock can catch. bypass them too. )
        
        //in home page we have div classed myAddArea which adblock can catch.So, bypass it too
        new ABBPage("/").AddAnotherKeysToBypass("myAddArea"),
        new ABBPage("/Home/Index").AddAnotherKeysToBypass("myAddArea")
    );
});


```
***Step 3***

Add UseAdBlockBypass to your application builder  (IApplicationBuilder)
`Startup.cs`
```csharp
app.UseAdBlockBypass();
```

***Step 4***

Add random parameter to your css and js which you want to bypass so that browser don't cache them

`MyAdd.cshtml`

```cshtml
@{
    var rnd = new Random();
    var randomValue = rnd.Next();//add random parameter to your css and js which you want to bypass so that browser don't cache them
}
<!--link your css,js file from new provider-->
<link href="/cssp/adblockme.css?v=@randomValue" rel="stylesheet" />
<link href="/cssp/myadd.css?v=@randomValue" rel="stylesheet" />

<div class="myTestClass2" data-test="asd-adds">
    <div id="myTestId">
        <div class="myTestClass">
            <div class="col-md-12" style="margin-bottom:10px">
                <div class="adBlockTest">
                    This is default value
                </div>
            </div>
        </div>
    </div>
</div>

<!--link your css,js file from new provider-->
<script src="/jsp/adblockme.js?v=@randomValue"></script>
```
After all, the uri of your css/js files and all the key values that you enter will be refreshed each time the cache time expires.
With this way your ads markers will be unpredictable. And you will able to bypass adblocks

Download and run ABB.Web project to test it.

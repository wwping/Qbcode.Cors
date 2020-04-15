# Qbcode.Cors
Bumblebee 网关的cors跨域插件
# 使用1
- Bumblebee 1.3.6.7 + 的版本增加了路由的跨域设置， 但是 路由聚合 和 自定义的api(比如 Qbcode.Configuration.Api)的跨域得不到解决
- 那么这个插件就适用在这两个场景下，如果没有适用 路由聚合 或者 类似 Qbcode.Configuration.Api这样的自定义的api的功能 那么这个插件是不必要的
# 使用2，
- 修改源码 
```
 在 Servers RequestAgent   OnReadResponseHeader
 找到  UrlRoute.Pluginer.HeaderWriting(Request, Response, mResponseHeader); 这一句 
 在这一句下面加一句 Response.Header.CopyTo(mResponseHeader);

 这时候这个插件就在全部请求中都起作用了
```
# 配置 
- 1，引用项目 或者引用 Qbcode.Cors.dll 
```
g = new Gateway(); 
.....省略一万个字 
g.LoadPlugin(
  typeof(Qbcode.Cors.Plugin).Assembly
);
```
- 3，在管理界面开启插件 
- 4，配置插件，将 Enabled改为true保存一下就生效了

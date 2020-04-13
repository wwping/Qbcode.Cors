# Qbcode.Cors
Bumblebee 网关的cors跨域插件，目前只能解决 路由聚合插件下的跨域问题，其它暂无办法
# 使用 
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

### Plugin.BingMap
Bienvenido al repositorio del plugin de bingmap para Xamarin Forms, actualmente estoy trabajando en este plugin, espero que si usted quiere apoyarme a codear las caracteristicas, seria genial! Igual puede mandarme mensaje para explicarle como es que esta desarrollado este plugin, usando un puente entre la Web y C#.

Caracteristica | Android | iOS | UWP
--- | --- | --- | --- 
Vista del mapa | :heavy_check_mark: | :heavy_check_mark: | :x:
Temas | :heavy_check_mark: | :heavy_check_mark: | :x:
Coleccion de chinchetas | :heavy_check_mark: | :heavy_check_mark: | :x:
Evento clic en chinchetas | :heavy_check_mark: | :heavy_check_mark: | :x:
Zoom exacta para ubicaciones personalizadas | :heavy_check_mark: | :heavy_check_mark: | :x:
Cambiar el centro del mapa y zoom | :heavy_check_mark: | :heavy_check_mark: | :x:
Colección de lineas sobre el mapa  | :heavy_check_mark: | :heavy_check_mark: | :x:
Evento clic en lineas sobre el mapa | :heavy_check_mark: | :heavy_check_mark: | :x:
Evento para detectar cambios del mapa | :heavy_check_mark: | :heavy_check_mark: | :x:

### Uso

Agrega la referencia a XAML
```
xmlns:BingMap="clr-namespace:Plugin.BingMap;assembly=Plugin.BingMap"
```

Agrega el control del mapa
```
<BingMap:Map ApiKey="{ApiKey}" Theme="Dark" />
```
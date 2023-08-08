# ToonBlastPrototype 
Proje ToonBlast Oyununun GamePlay Prototipidir.
Oyunun base sistemi Constructor Dependency Injection tasarım modeli kullanılarak yazılmıştır.
GridSpawnController,GridController,GridMoveController ve GridGoalController clasları LevelController'in Ctor'unda oluşturulmuş, LevelController ise GameManagerin Startında Oluşturulmuştur.
Sahne referansları ve ScriptableObject olarak class settings'leri GameManagerde tutulmuş ve ilgili classlara buradan dağıtılmıştır.

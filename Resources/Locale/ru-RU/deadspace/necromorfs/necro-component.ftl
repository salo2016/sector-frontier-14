# Necro ent
ent-MobNecromant = иссушитель звездных душ
    .desc = От него веет смертью и загадочностью.
    .suffix = Некроморф, Призрачная роль

ent-MobSlasher = расчленитель
    .desc = Некроморф похожий на мутировавший труп.
    .suffix = Некроморф, Призрачная роль
ent-MobSlasherB = { ent-MobSlasher }
    .desc = { ent-MobSlasher }
ent-MobSlasherSmall = { ent-MobSlasher }
    .desc = { ent-MobSlasher }

ent-MobPregnant = беременный
    .desc = Некроморф похожий на мутировавший труп.
    .suffix = Некроморф, Призрачная роль
ent-MobPregnantB = { ent-MobPregnant }
    .desc = { ent-MobPregnant.desc }

ent-MobBrute = зверь
    .desc = Поистине гигантский некроморф.
    .suffix = Некроморф, Призрачная роль
ent-MobBruteB = { ent-MobBrute }
    .desc = { ent-MobBrute.desc }

ent-MobInfector = заразитель
    .desc = Некроморф напоминающий своим внешним видом ската.
    .suffix = Некроморф, Призрачная роль
ent-MobInfectorB = { ent-MobInfector }
    .desc = { ent-MobInfector.desc }

ent-MobDivader = разделитель
    .desc = Некроморф похожий на мутировавший труп.
    .suffix = Некроморф, Призрачная роль
ent-MobDivaderB = { ent-MobDivader }
    .desc = { ent-MobDivader.desc }

ent-MobDivaderRH = рука разделителя
    .desc = Рука некроморфа разделителя, похоже она живёт своей жизнью.
    .suffix = Некроморф
ent-MobDivaderRHB = { ent-MobDivaderRH }
    .desc = { ent-MobDivaderRH.desc }

ent-MobDivaderLH = рука разделителя
    .desc = Рука некроморфа разделителя, похоже она живёт своей жизнью.
    .suffix = Некроморф
ent-MobDivaderLHB = { ent-MobDivaderLH }
    .desc = { ent-MobDivaderLH.desc }

ent-MobDivaderH = голова разделителя
    .desc = Голова некроморфа разделителя, похоже она живёт своей жизнью.
    .suffix = Некроморф
ent-MobDivaderHB = { ent-MobDivaderH }
    .desc = { ent-MobDivaderH.desc }

ent-MobCorpseCollector = собиратель трупов
    .desc = Какой же этот некроморф огромный!
    .suffix = Некроморф, Призрачная роль
ent-MobCorpseCollectorB = { ent-MobCorpseCollector }
    .desc = { ent-MobCorpseCollector.desc }

ent-MobTwitcher = охотник
    .desc = Некроморф похожий на мутировавший труп.
    .suffix = Некроморф, Призрачная роль
ent-MobTwitcherB = { ent-MobTwitcher }
    .desc = { ent-MobTwitcher.desc }

ent-MobTwitcherlvl2 = возвышенный
    .desc = Некроморф похожий на мутировавший труп.
    .suffix = Некроморф
ent-MobTwitcherlvl2B = { ent-MobTwitcherlvl2 }
    .desc = { ent-MobTwitcherlvl2.desc }

# Other ent
ent-ObeliskStoperProduct = { ent-CrateObeliskStoper }
    .desc = { ent-CrateObeliskStoper.desc }

ent-ZetaOneMedipenProduct = { ent-CrateZetaOneMedipen }
    .desc = { ent-CrateZetaOneMedipen.desc }

ent-StructureObelisk = красный обелиск
    .desc = От него веет смертью.

ent-StructureBlackObelisk = черный обелиск
    .desc = От него веет смертью.

ent-StructureObelisk2 = красный обелиск
    .desc = От него веет смертью.
    .suffix = Без оповещения

ent-StructureBlackObelisk2 = черный обелиск
    .desc = От него веет смертью.
    .suffix = Без оповещения

ent-FloorNecroTileItemFlesh = некротический пол
    .desc = От него дурно пахнет.

ent-NecroKudzu = некроморфные волокна
    .desc = Кажется они распространяются. Не опасны и уязвимы, но замедляют передвижение.

ent-NecroTentacle = некро щупальца
    .desc = Кажется они не распространяются. Выглядят очень опасными.

# Stoper

ent-ObeliskStoper = бета001
    .desc = Прототип, созданный на основе ксеноартефактов. Останавливает пси-влияние некрообелиск на некоторое время. Одноразовый.

ent-CrateObeliskStoper = ящик с бета001
    .desc = Останавливает некрообелиск на некоторое время.

# ZetaOne

ent-CrateZetaOneMedipen = ящик с ZetaOne
    .desc = Ящик с медипенами ZetaOne лечащими некроинфецию.

ent-ZetaOneMedipen = медипен ZetaOne
    .desc = Содержит в себе лекарство от некроинфеции.

reagent-name-zetaone = ZetaOne
reagent-desc-zetaone = На вкус как лекарство.

reagent-effect-guidebook-cure-infection-dead =
    { $chance ->
        [1] Лечит
       *[other] лечат
    } некроинфецию

# ZetaTwo

ent-ZetaTwoMedipen = медипен ZetaTwo
    .desc = Содержит в себе лекарство от некроинфеции и сильно действующий наркотик дезоксиэфедрин, восстанавливающий рассудок после пребывание возле обелиска.

reagent-effect-guidebook-cure-sanity =
    { $chance ->
        [1] Восстанавливает
       *[other] восстанавливает
    } рассудок после пребывания возле некро-обелиска

# Extract infector

ent-SyringeExtractInfectorDead = шприц
    .desc = { ent-BaseSyringe.desc }
    .suffix = Экстракт заразителя, НЕ МАППИТЬ

entity-name-extract-infector = экстракт заразителя
reagent-desc-extract-infector  = Густой и розовый, пахнет отвратительно. Пробовать не стану!

reagent-effect-guidebook-cause-infection-dead =
    { $chance ->
        [1] Заражает
       *[other] заражает
    } некроинфецией вызывающей обращение в некроморфа после смерти

# Serum enslaved

ent-SyringeSerumEnslaved = шприц
    .desc = { ent-BaseSyringe.desc }
    .suffix = Сыворотка порабощения, НЕ МАППИТЬ

entity-name-serum-enslaved = сыворотка порабощения
reagent-desc-serum-enslaved  = Густой и чёрный, пахнет ужасно.

reagent-effect-guidebook-cause-enslave =
    { $chance ->
        [1] Порабощает
       *[other] порабощает
    } жертву и делает её рабом юнитологов

# Necromant store

ent-ActionNecromantArmy = призыв расчленителя.
    .desc = призыв расчленителя.
ent-ActionNecromantPregnant = призыв беременного.
    .desc = призыв беременного.
ent-ActionNecromantTwitcher = призыв охотника.
    .desc = призыв охотника.
ent-ActionNecromantInfector = призыв заразителя.
    .desc = призыв заразителя.
ent-ActionNecromantBrute = призыв зверя.
    .desc = призыв зверя.
ent-ActionNecromantDivader = призыв раздетителя.
    .desc = призыв раздетителя.

action--slasher = Призыв расчленителя.
action-description-pregnant = Призыв беременного.
action-description-twitcher = Призыв охотника.
action-description-infector = Призыв заразителя.
action-description-brute = Призыв зверя.
action-description-divader = Призыв раздетителя.

list-name-slasher = некроморф
list-name-pregnant = беременный
list-name-twitcher = охотник
list-name-infector = заразитель
list-name-brute = зверь
list-name-divader = разделитель

list-description-slasher = Этой способностью вы сможете призвать рядового некроморфа.
list-description-pregnant = Этой способностью вы сможете призвать беременного.
list-description-twitcher = Этой способностью вы сможете призвать охотника.
list-description-infector = Этой способностью вы сможете призвать заразителя.
list-description-brute = Этой способностью вы сможете призвать зверя.
list-description-divader = Этой способностью вы сможете призвать раздетителя.

entity-name-shop-necromant = Магазин способностей.
entity-description-shop-necromant = Открыть магазин способностей.

# Technology

research-technology-basic-necro-research = Некротехнологии

# Actions

ent-ActionUnitologObeliskSpawn = Призвать обелиск
    .desc = Призвать обелиск находясь рядом с тремя порабощёнными и трупом гуманоида.

ent-ActionUnitologTentacleSpawn = Разместить некро щупальца
    .desc = Разместить щупальца некро, наносящие урон, но не распространяющиеся.

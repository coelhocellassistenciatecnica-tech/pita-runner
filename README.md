# 🎮 Pita Runner

> Jogo mobile 3D estilo Mob Control desenvolvido em Unity 6 + C#

---

## 📱 Sobre o Jogo

**Pita Runner** é um jogo mobile onde você controla uma multidão de personagens que corre automaticamente por uma pista. Escolha portais matemáticos para multiplicar seu exército e vença a batalha final contra o inimigo!

---

## 🛠️ Requisitos

- **Unity 6** (6000.x ou superior)
- **Plataformas**: Android, iOS
- **TextMeshPro** (incluído no Unity)
- **Universal Render Pipeline (URP)** — recomendado

---

## 📂 Estrutura do Projeto

```
PitaRunner/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           → GameManager, SaveManager, AudioManager, LevelManager
│   │   ├── Player/         → PlayerController, CameraFollow
│   │   ├── Crowd/          → CrowdManager, Unit (com Object Pooling + GPU Instancing)
│   │   ├── Portals/        → Portal, PortalSpawner
│   │   ├── Obstacles/      → ObstacleBase, RotatingSaw, Hammer, Wall, Cannon
│   │   ├── Battle/         → BattleManager, EnemyUnit, BaseController, BossController
│   │   ├── Economy/        → CurrencyManager, UpgradeManager
│   │   ├── Skins/          → SkinManager (6 skins: Soldado, Ninja, Cavaleiro, Robô, Futurista, Militar)
│   │   ├── Level/          → ProceduralLevelGenerator
│   │   ├── Effects/        → ParticleManager, DamageNumber (com pool)
│   │   └── UI/             → MainMenuUI, GameUI, VictoryUI, DefeatUI, ShopUI
│   ├── ScriptableObjects/  → PortalData, SkinData, UpgradeData, LevelData, BossData
│   ├── Prefabs/            → (adicione os prefabs aqui)
│   ├── Materials/          → (materiais do jogo)
│   ├── Textures/           → (texturas e sprites)
│   ├── Animations/         → (animações dos personagens)
│   ├── Audio/              → (músicas e efeitos sonoros)
│   ├── Art/
│   │   ├── Characters/     → Personagens do jogo
│   │   └── UI/             → Logo, ícone e artes de interface
│   └── Scenes/             → MainMenu, GameScene, ShopScene, SkinsScene
└── ProjectSettings/
```

---

## 🎯 Sistemas Implementados

### ✅ Core
- `GameManager` — Máquina de estados (MainMenu, Playing, Battle, Victory, Defeat, Paused)
- `SaveManager` — Persistência local via PlayerPrefs (JSON)
- `AudioManager` — Gerenciamento de música e SFX com pooling
- `LevelManager` — Controle de zonas e progressão de fase

### ✅ Multidão (Crowd System)
- `CrowdManager` — Object Pooling de até 1200+ unidades simultâneas
- `Unit` — Unidade individual com IA de batalha
- Formação automática em grade seguindo o jogador
- GPU Instancing pronto via MaterialPropertyBlock

### ✅ Jogador
- `PlayerController` — Corrida automática + controle horizontal por toque
- `CameraFollow` — Câmera suave com transição para modo batalha

### ✅ Portais
- `Portal` — Multiplicadores (x2, x3, x5) e adição (+10, +25, +50, +100)
- Portais negativos configuráveis
- `PortalSpawner` — Geração via ScriptableObject

### ✅ Obstáculos
- `RotatingSaw` — Serra giratória com oscilação opcional
- `Hammer` — Martelo pendular com pivô
- `Wall` — Muro destrutível com durabilidade
- `Cannon` + `CannonBullet` — Canhão com projéteis

### ✅ Sistema de Batalha
- `BattleManager` — Spawn de inimigos escalado por fase
- `EnemyUnit` — IA inimiga com perseguição e ataque
- `BaseController` — Base com vida, defesa e shake visual
- `BossController` — Chefe com 3 tipos: Robô Gigante, Monstro Mecânico, Tanque Blindado

### ✅ Economia
- `CurrencyManager` — Moedas com evento de atualização de UI
- `UpgradeManager` — 5 upgrades: Dano, Velocidade, Qtd. Inicial, Taxa Multiplicadora, Vida

### ✅ Skins
- `SkinManager` — Sistema completo com 6 skins e raridades
- Compra, desbloqueio e equipamento persistidos

### ✅ Geração Procedural
- `ProceduralLevelGenerator` — Geração aleatória de portais, obstáculos e decorações por fase
- Dificuldade progressiva automática

### ✅ Efeitos
- `ParticleManager` — Pool de partículas: hit, morte, multiplicação, explosão
- `DamageNumberPool` — Números de dano flutuantes animados

### ✅ UI Completa
- `MainMenuUI` — Tela inicial com configurações de áudio
- `GameUI` — HUD em tempo real (tropas, moedas, progresso)
- `VictoryUI` — Recompensa animada, contador de moedas
- `DefeatUI` — Dicas aleatórias, opção de reviver via Rewarded Ad
- `ShopUI` — Loja com upgrades e skins dinâmicos

---

## 🚀 Como Abrir no Unity

1. Abra o **Unity Hub**
2. Clique em **"Add project from disk"**
3. Selecione a pasta `PitaRunner/`
4. Confirme com **Unity 6**
5. Aguarde a importação de assets

### Configuração Inicial no Editor

1. **Criar as cenas**: `File > New Scene` → salve como `MainMenu`, `GameScene`, `ShopScene`, `SkinsScene` em `Assets/Scenes/`
2. **Adicionar tags**: `Edit > Project Settings > Tags` → adicione: `Player`, `Unit`, `Enemy`, `Portal`, `Obstacle`
3. **Criar o GameManager Object**: na cena, crie `GameObject > Empty` → adicione `GameManager`, `SaveManager`, `AudioManager`, `LevelManager`
4. **Configurar Player**: crie o prefab do jogador com tag `Player`, adicione `PlayerController` e `Rigidbody`
5. **TextMeshPro**: `Window > TextMeshPro > Import TMP Essential Resources`

---

## 📦 Dependências

| Pacote | Uso |
|--------|-----|
| TextMeshPro | Textos de UI e dano |
| URP (recomendado) | Renderização mobile otimizada |
| Unity Ads (futuro) | Monetização |

---

## 🎨 Assets de Arte

Coloque na pasta `Assets/Art/`:
- `Logo.png` — Logo do jogo
- `AppIcon.png` — Ícone 1024x1024
- `Characters/` — Sprites/modelos dos personagens

---

## 📱 Build Android

1. `File > Build Settings > Android`
2. `Switch Platform`
3. Configure `Player Settings`:
   - Package Name: `com.seuname.pitarunner`
   - Minimum API Level: 24
4. `Build And Run`

---

## 🔮 Roadmap Futuro

- [ ] Integração Firebase (login + ranking)
- [ ] Unity Ads (Rewarded + Interstitial)
- [ ] ECS/DOTS para 1000+ unidades
- [ ] Mais skins e bosses
- [ ] Eventos online sazonais

---

## 📄 Licença

Projeto privado — todos os direitos reservados.

---

*Desenvolvido com Unity 6 + C# | Pita Runner © 2025*

# TGameUnity
[![](https://img.shields.io/badge/made%20by-Alex%20Tang-blue.svg?style=flat-square)](https://protocol.ai)
[![](https://img.shields.io/badge/project-go-yellow.svg?style=flat-square)](https://libp2p.io/)
[![](https://img.shields.io/badge/freenode-%23libp2p-yellow.svg?style=flat-square)](https://webchat.freenode.net/?channels=%23libp2p)
[![codecov](https://codecov.io/gh/libp2p/go-reuseport/branch/master/graph/badge.svg)](https://codecov.io/gh/libp2p/go-reuseport)
[![Travis CI](https://travis-ci.org/libp2p/go-reuseport.svg?branch=master)](https://travis-ci.org/libp2p/go-reuseport)
[![Discourse posts](https://img.shields.io/discourse/https/discuss.libp2p.io/posts.svg)](https://discuss.libp2p.io)

### TGame客户端
#
### 使用Go + TCP + UDP +My

### doc: https://github.com/ALEXTANGXIAO/TGameUnity
### 服务端地址: https://github.com/ALEXTANGXIAO/TGameServer

Tested on `android`, `ios`, and `windows`.

---
```
项目结构

Assets
├── 3DBase       // 3D基础框架
├── Animations   // 动画资源
├── Resources    // Resources目录
├── Scenes       // 场景
└── Scripts      // 脚本资源

Scripts
├── FrameWork           // 基础框架
├── Games               // 核心逻辑
    ├── Actor           // 角色系统
    ├── ActorFight      // 角色战斗系统
    ├── ActorName       // ActorName
    ├── Bubble          // Bubble
    ├── Camera          // Camera
    ├── DataCenter      // 数据中心
    ├── Proto           // 协议相关
    └── UI              // UI
├── GameApp.cs          // 主入口
└── GameApp_RegisterSystem.cs      // 主入口注册系统
```
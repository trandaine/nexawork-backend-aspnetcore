Cấu trúc thư mục chuẩn:

``` Plaintext
src/
│
├── app/                # cấu hình global (router, store…)
│   ├── router.tsx
│   └── App.tsx
│
├── features/           # chia theo feature (QUAN TRỌNG)
│   ├── auth/
│   │   ├── api.ts
│   │   ├── authSlice.ts (nếu dùng redux)
│   │   ├── hooks.ts
│   │   ├── types.ts
│   │   └── components/
│   │       └── LoginForm.tsx
│   │
│   └── user/
│       ├── api.ts
│       ├── types.ts
│       └── components/
│
├── pages/              # các trang (route-level)
│   ├── LoginPage.tsx
│   ├── DashboardPage.tsx
│   └── NotFoundPage.tsx
│
├── components/         # component dùng chung
│   ├── ui/
│   │   ├── Button.tsx
│   │   └── Input.tsx
│   │
│   └── layout/
│       ├── Navbar.tsx
│       └── ProtectedRoute.tsx
│
├── hooks/              # custom hooks global
│   └── useAuth.ts
│
├── services/           # gọi API chung
│   ├── httpClient.ts
│   └── tokenService.ts
│
├── utils/              # helper functions
│   └── helpers.ts
│
├── types/              # global types
│   └── index.ts
│
└── assets/             # ảnh, icon, css

```

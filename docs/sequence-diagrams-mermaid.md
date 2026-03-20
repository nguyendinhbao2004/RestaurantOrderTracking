# RestaurantOrderTracking - Sequence Diagrams (Mermaid)

Tai lieu nay tong hop theo phong cach senior: co inventory chuc nang, luong happy case va unhappy case.

## 1) Functional Inventory (API surface)

- Auth
  - POST /api/Auth/login
  - POST /api/Auth/CreateAccount
  - POST /api/Auth/CreateWaiter
  - POST /api/Auth/CreateChef
  - POST /api/Auth/RegisterCustomer
- Account
  - GET /api/Account
- Area
  - GET /api/Area
  - POST /api/Area
  - PUT /api/Area
  - DELETE /api/Area/{id}
- Category
  - GET /api/Category
  - POST /api/Category
  - PUT /api/Category
  - DELETE /api/Category/{id}
- Product
  - GET /api/Product
  - POST /api/Product
  - PUT /api/Product/Update-Info
  - PUT /api/Product/Update-Status/{id}
- Table + QR Session
  - GET /api/Table
  - GET /api/Table/{id}
  - GET /api/Table/area/{areaId}
  - POST /api/Table
  - PUT /api/Table/update-info
  - PUT /api/Table/update-status
  - POST /api/Table/qr-session/{tableId}
  - PUT /api/Table/qr-session/{tableId}/refresh
  - GET /api/Table/by-session/{sessionToken}
- Order
  - GET /api/Order
  - GET /api/Order/{id}
  - POST /api/Order
  - PUT /api/Order/Update-Info
  - PUT /api/Order/Update-Status
  - POST /api/Order/online
- OrderItem
  - GET /api/OrderItem
  - POST /api/OrderItem
  - PUT /api/OrderItem/{orderItemId}/Update-Status
  - PUT /api/OrderItem/{orderItemId}/Update-Info
- Bill (Cashier)
  - GET /api/Cashier/bill
  - GET /api/Cashier/bill/{id}
  - POST /api/Cashier/bill
  - PUT /api/Cashier/bill/update
  - PUT /api/Cashier/bill/pay
  - PUT /api/Cashier/bill/cancel
- Payment (PayOS)
  - POST /api/Payment/create-link
  - GET /api/Payment/info/{orderCode}
  - POST /api/Payment/cancel
  - POST /api/Payment/webhook
  - POST /api/Payment/confirm-webhook
- Customer
  - GET /api/Customer/account/{accountId}
  - PUT /api/Customer/{id}
  - DELETE /api/Customer/{id}
- WorkSchedule
  - GET /api/WorkSchedule
  - POST /api/WorkSchedule
  - PUT /api/WorkSchedule
  - DELETE /api/WorkSchedule/{id}
  - PUT /api/WorkSchedule/CheckIn/{id}
  - PUT /api/WorkSchedule/CheckOut/{id}
- Dashboard
  - GET /api/Dashboard/summary

## 2) Global API Processing Template (ap dung cho tat ca endpoint)

```mermaid
sequenceDiagram
    autonumber
    actor UI as Client/App
    participant C as REST Controller
    participant H as Feature Handler
    participant R as Repository/UoW
    participant DB as PostgreSQL
    participant Hub as SignalR Hub

    UI->>C: HTTP request (REST/gRPC)
    C->>H: ExecuteFeature(command/query)

    alt Validation/Auth failure
        H-->>C: Result.Failure(error)
        C-->>UI: 400/401/403/404
    else Domain processing
        H->>R: Load/Save aggregate
        R->>DB: SQL + COMMIT
        DB-->>R: Result set

        alt Domain/Business failure
            H-->>C: Result.Failure(message)
            C-->>UI: 400/409
        else Success
            opt Need realtime update
                H->>Hub: NotifyRoleGroups(event)
                Hub-->>UI: SignalR push
            end
            H-->>C: Result.Success(data)
            C-->>UI: 200/201
        end
    end
```

## 3) Auth Login Flow (happy + unhappy)

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant AuthC as AuthController
    participant LoginH as LoginHandler
    participant AccRepo as AccountRepository
    participant WaiterRepo as WaiterRepository
    participant Jwt as JwtTokenService
    participant UoW as UnitOfWork
    participant DB as PostgreSQL

    User->>AuthC: POST /api/Auth/login (username,password)
    AuthC->>LoginH: Handle(LoginCommand)

    LoginH->>AccRepo: GetByUserNameAsync(username)
    AccRepo->>DB: SELECT account + role
    DB-->>AccRepo: user/null

    alt Unhappy - User not found
        LoginH-->>AuthC: Failure("Invalid username or password")
        AuthC-->>User: 400
    else User exists
        LoginH->>AccRepo: CheckPasswordAsync(user,password)

        alt Unhappy - Wrong password
            LoginH-->>AuthC: Failure("Invalid username or password")
            AuthC-->>User: 400
        else Password valid
            LoginH->>Jwt: GenerateToken + RefreshToken
            LoginH->>AccRepo: Update(user.AddRefreshToken)
            LoginH->>UoW: SaveChangesAsync
            UoW->>DB: COMMIT

            opt Role == Waiter
                LoginH->>WaiterRepo: GetByAccountIdAsync(user.Id)
                WaiterRepo->>DB: SELECT assigned area
                DB-->>WaiterRepo: waiter/none
            end

            LoginH-->>AuthC: Success(AuthResponse)
            AuthC-->>User: 200 + tokens
        end
    end
```

## 4) Order + OrderItem + Role-based WebSocket

```mermaid
sequenceDiagram
    autonumber
    actor Staff as Waiter/Chef/Cashier/Manager
    participant OC as OrderController
    participant OIC as OrderItemController
    participant OH as UpdateStatusOrderHandler
    participant OIH as UpdateStatusOrderItemHandler
    participant OR as OrderRepository
    participant OIR as OrderItemRepository
    participant LogR as OrderItemLogRepository
    participant TR as TableRepository
    participant UoW as UnitOfWork
    participant NS as NotificationService
    participant Hub as RestaurantHub
    participant DB as PostgreSQL

    Note over Staff,Hub: Client joins role groups via /hubs/restaurant for realtime updates

    par Update Order Status
        Staff->>OC: PUT /api/Order/Update-Status
        OC->>OH: Handle(UpdateStatusOrderCommand)
        OH->>OR: GetByIdAsync(orderId)
        OR->>DB: SELECT Order
        DB-->>OR: Order/null

        alt Unhappy - Order not found
            OH-->>OC: Failure("Order not found")
            OC-->>Staff: 400/404
        else Order found
            OH->>OH: order.UpdateStatus(newStatus)
            alt Unhappy - Invalid transition
                OH-->>OC: Failure(ex.Message)
                OC-->>Staff: 400
            else Valid
                opt Completed/Cancelled + has table
                    OH->>TR: GetByIdAsync(tableId)
                    TR->>DB: SELECT Table
                    DB-->>TR: Table
                    OH->>TR: SetAvailable + Update
                end
                OH->>UoW: SaveChangesAsync
                UoW->>DB: COMMIT
                OH->>NS: NotifyOrderStatusChanged(...)
                NS->>Hub: Push to role groups
                Hub-->>Staff: SignalR NotifyOrderStatusChanged
                OH-->>OC: Success
                OC-->>Staff: 200
            end
        end
    and Update OrderItem Status
        Staff->>OIC: PUT /api/OrderItem/{id}/Update-Status
        OIC->>OIH: Handle(UpdateStatusOrderItemCommand)
        OIH->>OIR: GetByIdAsync(orderItemId)
        OIR->>DB: SELECT OrderItem
        DB-->>OIR: OrderItem/null

        alt Unhappy - OrderItem not found
            OIH-->>OIC: Failure("OrderItem ... not found")
            OIC-->>Staff: 400/404
        else Found
            OIH->>OIH: UpdateStatus (domain sequential rule)
            alt Unhappy - Invalid transition
                OIH-->>OIC: Failure(ex.Message)
                OIC-->>Staff: 400
            else Valid
                opt Confirmed->Cooking without assignee
                    OIH-->>OIC: Failure("AssigneeId is required")
                    OIC-->>Staff: 400
                else Assignee OK
                    OIH->>LogR: AddAsync(OrderItemLog)
                    OIH->>UoW: SaveChangesAsync
                    UoW->>DB: COMMIT
                    OIH->>NS: NotifyOrderStatusChanged(orderId,...)
                    NS->>Hub: Push to role groups
                    Hub-->>Staff: SignalR NotifyOrderStatusChanged
                    OIH-->>OIC: Success
                    OIC-->>Staff: 200
                end
            end
        end
    end
```

## 5) Payment Flow (PayOS + Webhook + Bill/Order/Table)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant PC as PaymentController
    participant CLH as CreatePaymentLinkHandler
    participant WH as ProcessWebhookHandler
    participant PS as PayOSService
    participant PTR as PaymentTransactionRepository
    participant BR as BillRepository
    participant OR as OrderRepository
    participant TR as TableRepository
    participant QRR as QRSessionRepository
    participant UoW as UnitOfWork
    participant DB as PostgreSQL

    Client->>PC: POST /api/Payment/create-link
    PC->>CLH: Handle(CreatePaymentLinkCommand)
    CLH->>PS: CreatePaymentLinkAsync(...)

    alt Unhappy - Provider/API error
        PS-->>CLH: exception/failure
        CLH-->>PC: Failure
        PC-->>Client: 400/500
    else Happy
        CLH->>PTR: Save transaction pending
        CLH->>UoW: SaveChanges
        UoW->>DB: COMMIT
        CLH-->>PC: Success(link)
        PC-->>Client: 200
    end

    Note over Client,DB: PayOS sends asynchronous webhook callback

    Client->>PC: POST /api/Payment/webhook (payload + signature)
    PC->>WH: Handle(ProcessWebhookCommand)
    WH->>PS: VerifyAndExtractWebhookData(payload)

    alt Unhappy - Invalid signature or failed payment
        WH-->>PC: Failure("invalid webhook signature")
        PC-->>Client: 400
    else Signature valid
        WH->>PTR: GetByOrderCodeAsync
        PTR->>DB: SELECT transaction
        DB-->>PTR: tx/null

        alt Unhappy - Transaction not found
            WH-->>PC: Failure("transaction not found")
            PC-->>Client: 404/400
        else Found tx
            WH->>BR: GetByIdWithDetailsAsync(tx.BillId)
            BR->>DB: SELECT bill + order
            DB-->>BR: bill/null

            alt Unhappy - Bill not found
                WH-->>PC: Failure("bill not found")
                PC-->>Client: 404/400
            else Bill found
                WH->>WH: Mark transaction PAID, update bill paid
                WH->>OR: Update order to Completed when allowed
                opt DineIn and order completed
                    WH->>TR: SetAvailable
                    WH->>QRR: Revoke old sessions + create new session
                end
                WH->>UoW: SaveChanges
                UoW->>DB: COMMIT
                WH-->>PC: Success("webhook processed")
                PC-->>Client: 200
            end
        end
    end
```

## 6) Table QR Session Lifecycle (Generate/Refresh/Resolve)

```mermaid
sequenceDiagram
    autonumber
    actor StaffOrCustomer as Staff/Customer App
    participant TC as TableController
    participant GQ as GenerateQRSessionHandler
    participant RQ as RefreshQRSessionHandler
    participant BQ as GetTableBySessionTokenHandler
    participant TR as TableRepository
    participant QRR as QRSessionRepository
    participant QRS as QRCodeService
    participant UoW as UnitOfWork
    participant DB as PostgreSQL

    StaffOrCustomer->>TC: POST /api/Table/qr-session/{tableId}
    TC->>GQ: Handle(GenerateQRSessionCommand)
    GQ->>TR: GetByIdAsync(tableId)
    TR->>DB: SELECT table
    DB-->>TR: table/null

    alt Unhappy - Table not found
        GQ-->>TC: Failure("Table not found")
        TC-->>StaffOrCustomer: 404/400
    else Happy
        GQ->>QRR: Revoke old active sessions
        GQ->>QRR: Add new QRSession
        GQ->>TR: Update table.QRCode
        GQ->>UoW: SaveChanges
        UoW->>DB: COMMIT
        GQ->>QRS: GenerateBase64(baseUrl?session=token)
        GQ-->>TC: Success(QR image + token + expiresAt)
        TC-->>StaffOrCustomer: 200
    end

    StaffOrCustomer->>TC: PUT /api/Table/qr-session/{tableId}/refresh
    TC->>RQ: Handle(RefreshQRSessionCommand)
    RQ->>QRR: Validate + revoke old session
    RQ->>UoW: SaveChanges
    UoW->>DB: COMMIT
    RQ-->>TC: Success(new session)
    TC-->>StaffOrCustomer: 200

    StaffOrCustomer->>TC: GET /api/Table/by-session/{sessionToken}
    TC->>BQ: Handle(GetTableBySessionTokenQuery)
    BQ->>QRR: Validate token

    alt Unhappy - Token invalid/expired/revoked
        BQ-->>TC: Failure
        TC-->>StaffOrCustomer: 400/404
    else Happy
        BQ->>TR: Load table
        TR->>DB: SELECT table
        DB-->>TR: table
        BQ-->>TC: Success(table info)
        TC-->>StaffOrCustomer: 200
    end
```
```

## 7) Notes for engineering governance

- Role-based WebSocket:
  - Hub add connection vao group role:{role} khi connect.
  - Notification service cho phep targetRoles linh hoat theo feature.
- Unhappy case conventions:
  - Input/domain rule failure -> Result.Failure -> 400 class response.
  - Not found -> Result.Failure (co the map 404 o controller neu can).
  - External provider fail -> log + failure response, khong commit partial state.
- Transaction boundary:
  - Handler commit qua UoW sau khi aggregate state hop le.
  - Webhook/payment flow la idempotency-sensitive, can bo sung idempotent keys neu scale cao.

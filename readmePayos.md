# Tài liệu Tích hợp API PayOS - Link Thanh Toán & Webhook

Tài liệu này tổng hợp toàn bộ các API dùng để tạo, quản lý Link thanh toán (Payment Link) và cấu hình Webhook nhận biến động số dư từ PayOS.

## 🔐 Xác thực chung (Authorizations)
Các API yêu cầu truyền Header xác thực bao gồm:
* `x-client-idx-api-key`
* `x-api-key`: API Key của kênh (Lấy tại giao diện My payOS).
* `x-client-id`: Client Id của kênh (Lấy tại giao diện My payOS).

---

## 1. Tạo link thanh toán
API dùng để tạo link thanh toán đơn hàng.

* **Phương thức:** `POST`
* **Endpoint:** `/v2/payment-requests`

### Headers
* `x-partner-code` (string, optional): Partner Code tham gia chương trình tích hợp đối tác payOS.

### Request Body (application/json)
| Field | Type | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `orderCode` | integer | **Yes** | Mã đơn hàng. |
| `amount` | integer | **Yes** | Số tiền thanh toán. |
| `description` | string | **Yes** | Mô tả thanh toán. Nếu tài khoản không liên kết qua payOS, giới hạn 9 ký tự. |
| `cancelUrl` | string <uri> | **Yes** | URL nhận dữ liệu khi người dùng chọn Huỷ đơn. |
| `returnUrl` | string <uri> | **Yes** | URL nhận dữ liệu khi đơn hàng thanh toán thành công. |
| `signature` | string | **Yes** | Chữ ký kiểm tra thông tin. |
| `buyerName` | string | No | Tên người mua hàng (cho Hóa đơn điện tử). |
| `buyerCompanyName`| string | No | Tên đơn vị mua hàng (cho Hóa đơn điện tử). |
| `buyerTaxCode` | string | No | Mã số thuế đơn vị mua (cho Hóa đơn điện tử). |
| `buyerAddress` | string | No | Địa chỉ đơn vị mua (cho Hóa đơn điện tử). |
| `buyerEmail` | string <email>| No | Email người mua hàng (cho Hóa đơn điện tử). |
| `buyerPhone` | string | No | Số điện thoại người mua (cho Hóa đơn điện tử). |
| `items` | Array of objects| No | Danh sách các sản phẩm thanh toán. Mỗi object gồm: `name`, `quantity`, `price`, `unit`, `taxPercentage`. |
| `invoice` | object | No | Thông tin hóa đơn. Gồm `buyerNotGetInvoice` (boolean) và `taxPercentage` (number). |
| `expiredAt` | number | No | Thời gian hết hạn của link (Unix Timestamp, Int32). |

### Responses
#### `200` Thành công (Response Schema: application/json)
*Lưu ý: HTTP 200 có thể chứa các lỗi logic sau:*
`SUCCESS`, `AMOUNT_NOT_INTEGER`, `DECIMAL_PART_TOO_LONG`, `ORDER_FOUND`, `VIETQR_PRO_CREATE_ORDER_FAIL`, `PAYMENT_GATEWAY_NOT_FOUND`, `PAYMENT_GATEWAY_PAUSED`, `BANK_INFO_NOT_FOUND`, `FI_SERVICE_ACCOUNT_STATE_INACTIVE`, `PAYMENT_GATEWAY_ORGANIZATION_NOT_FOUND`, `INVALID_PARTNER_CODE`, `INVALID_CONTENT_TYPE`, `INVALID_PARAM`, `PAYMENT_REQUEST_DATA_SIGNATURE_INCORRECT`, `SUBSCRIPTION_NOT_FOUND`, `BALANCE_NOT_ENOUGH`.

* **code** (string, required): Mã lỗi.
* **desc** (string, required): Thông tin lỗi.
* **data** (object, required): Chứa dữ liệu trả về (`bin`, `accountNumber`, `accountName`, `amount`, `description`, `orderCode`, `currency`, `paymentLinkId`, `status`, `checkoutUrl`, `qrCode`).
* **signature** (string, required): Chữ ký kiểm tra tính toàn vẹn của response.

#### `401` Unauthorized
* **code**, **desc**
#### `429` Too Many Request

---

## 2. Lấy thông tin link thanh toán
API dùng để lấy thông tin của link thanh toán.
*Lưu ý: Thông tin đối ứng `counterAccount` chỉ hỗ trợ MB Bank, ACB, KienlongBank.*

* **Phương thức:** `GET`
* **Endpoint:** `/v2/payment-requests/{id}`

### Responses
#### `200` Thành công (Response Schema: application/json)
*Lỗi logic có thể gặp:* `SUCCESS`, `INVALID_PARAM`, `PAYMENT_LINK_NOT_FOUND`, `PAYMENT_GATEWAY_NOT_FOUND`, `PAYMENT_GATEWAY_PAUSED`, `BANK_INFO_NOT_FOUND`, `FI_SERVICE_ACCOUNT_STATE_INACTIVE`, `PAYMENT_GATEWAY_ORGANIZATION_NOT_FOUND`.

* **code**, **desc**, **signature**.
* **data** (object, required): Chứa chi tiết đơn hàng (`id`, `orderCode`, `amount`, `amountPaid`, `amountRemaining`, `status`, `createdAt`, `transactions`).

---

## 3. Huỷ link thanh toán
API dùng để hủy link thanh toán.

* **Phương thức:** `POST`
* **Endpoint:** `/v2/payment-requests/{id}/cancel`

### Request Body
* `cancellationReason` (string, optional): Lý do hủy.

### Responses
#### `200` Thành công (Response Schema: application/json)
*Lỗi logic có thể gặp:* `SUCCESS`, `INVALID_PARAM`, `PAYMENT_LINK_NOT_FOUND`, `PAYMENT_LINK_CANNOT_BE_CANCELED`, `PAYMENT_GATEWAY_NOT_FOUND`...
* **code**, **desc**, **signature**.
* **data** (object, required): Dữ liệu đơn hàng sau khi hủy (thêm `canceledAt`, `cancellationReason`).

---

## 4. Lấy thông tin hóa đơn
* **Phương thức:** `GET`
* **Endpoint:** `/v2/payment-requests/{id}/invoices`

### Responses
#### `200` Thành công (Response Schema: application/json)
* **code**, **desc**, **signature**.
* **data** (object): Chứa mảng `invoices` (mỗi phần tử gồm: `invoiceId`, `invoiceNumber`, `issuedTimestamp`, `issuedDatetime`, `transactionId`, `reservationCode`, `codeOfTax`).

---

## 5. Tải hóa đơn
* **Phương thức:** `GET`
* **Endpoint:** `/v2/payment-requests/{id}/invoices/{invoice-id}/download`

### Responses
#### `200` Thành công
* **Response Headers:** `Content-Disposition`, `Content-Type: application/pdf`
* **Response Schema:** File PDF nội dung hóa đơn (binary).

---

## 6. Webhook thanh toán (Nhận thông tin)
Webhook của cửa hàng dùng để nhận dữ liệu thanh toán từ payOS khi có giao dịch thành công.

* **Phương thức:** `POST` (Từ payOS gửi tới Server của bạn)
* **Request Body (application/json) từ payOS:**
  * `code`, `desc`, `success`.
  * `data` (object, required): Chứa thông tin giao dịch (`orderCode`, `amount`, `description`, `accountNumber`, `reference`, `transactionDateTime`, `currency`, `paymentLinkId`, `counterAccountBankId`, `counterAccountBankName`, `counterAccountName`, `counterAccountNumber`, `virtualAccountName`, `virtualAccountNumber`).
  * `signature` (string, required): Chữ kí để kiểm tra tính toàn vẹn của thông tin.

---

## 7. Kiểm tra và thêm hoặc cập nhật Webhook URL
API dùng để xác thực và cập nhật Webhook URL.

* **Phương thức:** `POST`
* **Endpoint:** `/confirm-webhook`

### Request Body
* `webhookUrl` (string, required): Đường dẫn nhận Webhook của bạn.

### Responses
#### `200` Thành công
* **code**, **desc**.
* **data** (object): Chứa `webhookUrl`, `accountNumber`, `accountName`, `name`, `shortName`.
#### Các lỗi khác: `400` (URL invalid), `401` (Missing Key), `5XX` (Lỗi hệ thống của bạn).

---

## 8. Hướng dẫn Kiểm tra Chữ ký (Signature Verification)

Để đảm bảo dữ liệu gửi đến và nhận về không bị giả mạo bởi hacker trong quá trình truyền tải, hệ thống PayOS sử dụng chữ ký điện tử mã hóa bằng thuật toán **HMAC_SHA256**.

⚠️ **CẢNH BÁO QUAN TRỌNG:** Cách tạo chữ ký của luồng Nhận tiền (payment-requests) và luồng Chi tiền (payouts) là **HOÀN TOÀN KHÁC NHAU**. Vui lòng đọc kỹ quy tắc dưới đây:

### 8.1. Đối với API Tạo Link Thanh Toán & Webhook (payment-requests)

* **Thuật toán:** `HMAC_SHA256`
* **Checksum Key:** Sử dụng Checksum Key được tạo ra sau khi bạn **tạo cổng thanh toán thành công** trên trang quản trị.
* **Quy tắc nối chuỗi tạo Data:**
    1.  Dữ liệu cần tạo chữ ký phải được sắp xếp theo thứ tự **Alphabet của Tên trường (Key)**.
    2.  Nối các trường lại với nhau theo định dạng: `key1=value1&key2=value2&...`
    3.  *Ví dụ:* `amount=10000&cancelUrl=...&description=...&orderCode=...&returnUrl=...`
* **Cấu trúc hàm mã hóa:** `hash_hmac("sha256", chuỗi_data_đã_nối, checksum_key)`

### 8.2. Đối với API Lệnh Chi (payouts)

* **Thuật toán:** `HMAC_SHA256`
* **Checksum Key:** Sử dụng Checksum Key được sinh ra khi bạn **tạo kênh chuyển tiền thành công** (Có thể thay đổi key này trên my.payos.vn).
* **Quy tắc nối chuỗi tạo Data (Phức tạp hơn):**
    1.  **Sắp xếp:** Các Key (tên trường) phải được sắp xếp theo thứ tự Alphabet. Tuy nhiên, nếu dữ liệu là một Mảng (Array), **bắt buộc phải giữ nguyên thứ tự các phần tử trong mảng** (không được đảo lộn mảng).
    2.  **Xử lý giá trị Rỗng:** Bất kỳ giá trị nào là `null` hoặc `undefined` đều phải được chuyển đổi thành chuỗi rỗng `""`.
    3.  **Mã hóa URI (Quan trọng):** Không nối trực tiếp giá trị vào chuỗi, mà phải bọc giá trị đó qua hàm Encode URI (ví dụ trong Javascript là `encodeURIComponent`).
    4.  Định dạng chuỗi cuối cùng: `key1=encodeURI(value1)&key2=encodeURI(value2)&...`
* **Cấu trúc hàm mã hóa:** `hash_hmac("sha256", chuỗi_data_đã_nối_và_encode, checksum_key)`
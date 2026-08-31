# FreshMart Features

## 1. Customer Authentication

FreshMart provides customer account management using ASP.NET Core Identity.

### Available

* Customer registration
* Customer login
* Customer logout
* Customer profile
* Authenticated-page protection
* Account lockout
* Password policy enforcement

---

## 2. Product Browsing

Customers can browse available grocery products.

### Available

* Product listing
* Product categories
* Product information
* Product price
* Product stock availability
* Product images
* Product availability status

Current categories include:

* Fruits
* Vegetables
* Dairy
* Bakery
* Snacks

---

## 3. Shopping Cart

Authenticated customers can manage their shopping cart.

### Available

* Add products to cart
* Increase quantity
* Decrease quantity
* Remove product
* Clear cart
* View item count
* Calculate subtotal
* Calculate delivery charge
* Calculate total

### Stock Validation

When increasing a cart quantity, FreshMart checks:

```text
Product exists
      ↓
Product is available
      ↓
Requested quantity is within stock
      ↓
Quantity increased
```

The cart is maintained using ASP.NET Core Session.

---

## 4. Checkout

Checkout is available to authenticated customers.

### Customer Information

The checkout form validates:

* Full name
* Phone number
* Address
* City
* Pincode
* Payment method

### Delivery Charges

```text
Subtotal >= ₹500 → Free delivery

Subtotal < ₹500  → ₹40 delivery
```

The final amount is:

```text
Total = Subtotal + Delivery Charge
```

---

## 5. Order Management

FreshMart creates an order from the customer's cart during checkout.

An order contains:

* Customer
* Order date
* Delivery information
* Subtotal
* Delivery charge
* Total
* Order status
* Order items

Each order item stores:

* Product ID
* Product name
* Product price
* Quantity

This preserves the product information associated with the order.

---

## 6. Inventory Management

Inventory is connected to the checkout process.

FreshMart validates:

* Product existence
* Product availability
* Available stock

During successful checkout:

```text
Order Created
     ↓
Stock Validated
     ↓
Stock Deducted
```

Stock changes are performed within the checkout database transaction.

---

## 7. Order Cancellation

Customers can cancel orders with the following statuses:

```text
Pending
Confirmed
```

When an eligible order is cancelled:

```text
Order → Cancelled
       +
Stock → Restored
```

Orders that are not in an eligible state cannot be cancelled through the customer cancellation flow.

---

## 8. Payment

FreshMart currently implements a simulated payment workflow.

Supported payment states:

```text
Pending
Paid
Failed
Expired
```

### Payment Creation

A payment is created when the order is created.

Initial state:

```text
Order   → Pending
Payment → Pending
```

The payment amount is based on the order total.

---

## 9. Payment Success

A successful simulated payment:

1. Changes payment status to `Paid`
2. Generates a transaction ID
3. Records the payment date
4. Changes the order status to `Confirmed`
5. Redirects the customer to order confirmation

Flow:

```text
Pending
   ↓
Paid
   ↓
Order Confirmed
   ↓
Order Confirmation
```

---

## 10. Payment Failure and Retry

A payment can enter the `Failed` state.

When payment fails:

```text
Payment → Failed
Order   → Pending
```

The customer can return to the payment page and retry the payment.

A previously paid payment cannot be changed back to failed.

---

## 11. Payment Expiration

Pending payments have an expiration timestamp.

The current implementation creates the payment with a **15-minute expiration period**.

If the payment expires:

```text
Payment → Expired
Order   → Cancelled
```

An expired payment cannot be processed as a normal payment or changed into a failed payment.

---

## 12. Duplicate Payment Protection

The payment flow checks whether the payment has already been marked as `Paid`.

If a customer attempts to process an already-paid payment again, the application redirects to the order confirmation instead of creating another payment transaction.

This provides basic duplicate-payment protection.

Further idempotency testing is documented separately in:

```text
docs/PAYMENT.md
```

---

## 13. Order Confirmation

After successful payment, customers are redirected to the order confirmation page.

The order is retrieved using:

```text
Order ID
+
Authenticated User ID
```

This prevents a customer from retrieving another customer's order through the order ID alone.

---

## 14. Customer Order History

Authenticated customers can view their previous orders.

Orders are:

* Associated with the logged-in user
* Sorted by newest order first
* Displayed with their order items

Customers can also cancel eligible orders from the order history.

---

## 15. Admin Dashboard

FreshMart provides a separate admin dashboard.

Admin access requires the `Admin` role.

The dashboard currently displays:

* Total products
* Total orders
* Total registered customers
* Total sales
* Five most recent orders

---

## 16. Admin Order Management

Administrators can manage customer orders.

The application validates administrative order-status updates to prevent unsupported status transitions.

Payment information associated with orders is also available to the admin order view.

---

## 17. Role-Based Access

FreshMart uses two application roles:

```text
Admin
Customer
```

Protected customer pages require authentication.

Administrative pages require:

```text
Admin role
```

This prevents normal customers from accessing administrative functionality.

---

## 18. Data Consistency Features

FreshMart includes several mechanisms intended to maintain consistent application state.

### Database Transaction

Checkout uses a database transaction for:

```text
Order creation
+
Order items
+
Stock deduction
```

### Optimistic Concurrency

Products use an EF Core row-version concurrency token.

This allows conflicting stock updates to be detected.

### Validation

The application validates:

* Customer input
* Product availability
* Product stock
* Payment state
* Order state
* User ownership


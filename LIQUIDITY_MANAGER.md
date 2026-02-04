# 💧 NexusFinance - Liquidity Manager Module

## Comprehensive Debt & Receivables Tracking

---

## 🎯 Overview

**Purpose:** Track money flow obligations - what you owe (Payables) and what's owed to you (Receivables). Calculate projected liquidity to prevent cash flow crises.

**Key Features:**
- ⚠️ **Payables (Liabilities)** - Money you owe to creditors
- 💰 **Receivables (Assets)** - Money owed to you by debtors
- 📈 **Liquidity Forecast** - Projected balance calculation
- 🔴 **Urgency Tracking** - Overdue, critical, high, medium, low priorities
- 📊 **Probability Weighting** - Receivables adjusted by confidence level

---

## 📐 Data Models

### **Payable** (`Models/LiquidityModels.cs`)

```csharp
public class Payable
{
    public string Id { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string CreditorName { get; set; }     // Who to pay
    public DateTime DueDate { get; set; }
    public bool IsPaid { get; set; }
    
    // Computed properties
    public string UrgencyLevel { get; }          // Overdue, Critical, High, Medium, Low
    public string UrgencyColor { get; }          // #D50000 (red) to #404040 (low)
}
```

**Urgency Calculation:**
- **Overdue:** `DueDate < Now` → `#D50000` (Critical Red)
- **Critical:** `≤ 3 days` → `#FF5252` (High Red)
- **High:** `≤ 7 days` → `#909090` (Silver)
- **Medium:** `≤ 14 days` → `#606060` (Dark Grey)
- **Low:** `> 14 days` → `#404040` (Very Dark Grey)
- **Paid:** → `#00C853` (Green)

### **Receivable** (`Models/LiquidityModels.cs`)

```csharp
public class Receivable
{
    public string Id { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string DebtorName { get; set; }       // Who owes you
    public string? ProjectId { get; set; }       // Optional project link
    public DateTime ExpectedDate { get; set; }
    public ProbabilityLevel Probability { get; set; }
    public bool IsReceived { get; set; }
    
    // Computed properties
    public decimal WeightedAmount { get; }       // Adjusted by probability
    public string ProbabilityColor { get; }
    public string ProbabilityText { get; }
}

public enum ProbabilityLevel
{
    Confirmed,    // 100% - Contract signed, work delivered
    Likely,       // 75% - Verbal agreement
    Uncertain     // 40% - Speculative, proposal stage
}
```

**Weighted Amount Calculation:**
```csharp
WeightedAmount = Probability switch
{
    Confirmed => Amount * 1.0,
    Likely => Amount * 0.75,
    Uncertain => Amount * 0.4
}
```

---

## 🖥️ UI Implementation

### **Liquidity Forecast Bar** (Top Widget)

**Formula Display:**
```
Current Cash + Pending Receivables - Pending Payables = Projected Balance
```

**Visual Encoding:**
- **Projected Balance > 0** → Green `#00C853`
- **Projected Balance < 0** → Red `#FF5252` (Warning!)

**Alert Indicators:**
- **Overdue Payables Count** → Red badge
- **Critical/High Payables Count** → Grey badge

**Example:**
```
₽450,000  +  ₽180,000  -  ₽250,000  =  ₽380,000 (Green)
Current      Receivables   Payables    SAFE
```

---

### **Split View Layout**

#### **Left Panel: OBLIGATIONS (I Owe) ⚠️**

**Card Structure:**
```
┌─────────────────────────────────────────┐
│ Tax Bill                    [CRITICAL]  │ ← Urgency badge
│ Federal Tax Service                     │
│                                         │
│ ₽125,000              Due: Feb 10, 2026│
│                       2d left           │
│                                         │
│ [✅ Mark Paid]  [✏️]  [🗑️]            │
└─────────────────────────────────────────┘
```

**Features:**
- Border color matches urgency level
- Amount in red `#FF5252`
- "Mark Paid" button (green background)
- Edit/Delete buttons (transparent)

#### **Right Panel: RECEIVABLES (Owed to Me) 💰**

**Card Structure:**
```
┌─────────────────────────────────────────┐
│ Freelance Project X      [CONFIRMED]    │ ← Probability badge
│ Client Corp LLC                         │
│                                         │
│ ₽350,000              Expected:         │
│ 100% confidence       Feb 15, 2026      │
│                                         │
│ [✅ Mark Received]  [✏️]  [🗑️]        │
└─────────────────────────────────────────┘
```

**Features:**
- Border color matches probability
- Amount in green `#00C853`
- Confidence percentage display
- "Mark Received" button

---

## 🔧 CRUD Operations

### **Payables Management**

**Add Payable:**
```csharp
[RelayCommand]
private void AddPayable()
{
    var dialog = new PayableEditorDialog();
    if (dialog.ShowDialog() == true)
    {
        _dataService.AddPayable(dialog.Result);
        LoadLiquidityData();
    }
}
```

**Dialog Fields:**
- Title/Description (required)
- Creditor Name (required)
- Amount (₽, required)
- Due Date (DatePicker)

**Edit/Delete:**
- Edit: Loads existing data into dialog
- Delete: Confirmation prompt
- Mark Paid: Sets `IsPaid = true`, removes from active list

### **Receivables Management**

**Add Receivable:**
```csharp
[RelayCommand]
private void AddReceivable()
{
    var projects = _dataService.GetProjects().ToList();
    var dialog = new ReceivableEditorDialog(projects);
    if (dialog.ShowDialog() == true)
    {
        _dataService.AddReceivable(dialog.Result);
        LoadLiquidityData();
    }
}
```

**Dialog Fields:**
- Title/Description (required)
- Debtor Name (required)
- Amount (₽, required)
- Related Project (optional dropdown)
- Probability (Confirmed/Likely/Uncertain)
- Expected Date (DatePicker)

---

## 💾 Data Persistence

**File:** `%APPDATA%\NexusFinance\data.json`

**Structure:**
```json
{
  "Payables": [
    {
      "Id": "guid",
      "Title": "Server Invoice - AWS",
      "Amount": 12500.0,
      "CreditorName": "Amazon Web Services",
      "DueDate": "2026-02-15T00:00:00",
      "IsPaid": false,
      "CreatedAt": "2026-02-01T10:30:00"
    }
  ],
  "Receivables": [
    {
      "Id": "guid",
      "Title": "Milestone Payment - NexusAI",
      "Amount": 250000.0,
      "DebtorName": "Tech Corp Inc",
      "ProjectId": "NexusAI",
      "ExpectedDate": "2026-02-20T00:00:00",
      "Probability": "Confirmed",
      "IsReceived": false,
      "CreatedAt": "2026-02-01T11:00:00"
    }
  ]
}
```

---

## 📊 Use Cases

### **Scenario 1: Cash Flow Crisis Prevention**

**Situation:** User has ₽100,000 current cash, ₽150,000 in payables due next week.

**Forecast Calculation:**
```
₽100,000 (Cash) + ₽50,000 (Receivables, 75% likely) - ₽150,000 (Payables)
= ₽0 (CRITICAL!)
```

**UI Alert:**
- Projected Balance shows **₽0** in RED
- Alert: "2 Critical Payables"
- User action: Contact debtor to accelerate receivables or negotiate payment extension

### **Scenario 2: Receivable Risk Assessment**

**Portfolio:**
- Receivable A: ₽500,000 (Confirmed) → Weighted: ₽500,000
- Receivable B: ₽300,000 (Likely) → Weighted: ₽225,000
- Receivable C: ₽200,000 (Uncertain) → Weighted: ₽80,000

**Total Expected:** ₽805,000 (realistic expectation, not ₽1,000,000)

### **Scenario 3: Project Milestone Tracking**

**Use Case:** Freelancer tracks client payments by project.

**Receivables:**
- "NexusAI Phase 1" → ₽250,000 (Confirmed, due Feb 20)
- "NexusAI Phase 2" → ₽300,000 (Likely, due Mar 15)
- "FinSync Consulting" → ₽150,000 (Confirmed, due Feb 25)

**Grouped View:** All NexusAI receivables = ₽550,000 weighted

---

## 🎨 Monochrome Stealth Glass Design

### **Color Palette:**

**Backgrounds:**
- Main surface: `#CC252526` (semi-transparent dark grey)
- Cards: `#1E1E1E` (solid dark)
- Forecast bar: `#CC252526` with glass effect

**Borders:**
- Normal: `#505050` (thin grey, 1px)
- Urgency/Probability: Dynamic (red to grey scale)

**Text:**
- Headers: `#E0E0E0` (Titanium Silver)
- Labels: `#AAAAAA` (Light Grey)
- Values: `#C0C0C0` to `#909090` (Silver to Mid Grey)

**Functional Colors:**
- Positive (Receivables): `#00C853` (Green)
- Negative (Payables): `#FF5252` or `#D50000` (Red)
- Warning (Low Balance): `#FF5252` (Red)

### **Glass Effects:**

**Drop Shadow:**
```xaml
<DropShadowEffect Color="Black" ShadowDepth="4" BlurRadius="10" Opacity="0.5"/>
```

**Semi-transparency:**
```xaml
Background="#CC252526"  <!-- 80% opacity dark grey -->
```

---

## 🚀 Navigation Integration

**Sidebar Button:**
- Icon: 💧 (Liquidity/Water drop)
- Label: "Liquidity"
- Command: `NavigateToLiquidityCommand`
- Active state: Grey background `#CC2A2A2A`, Silver border

**Keyboard Shortcut:** (Future) `Alt+L`

---

## 🔍 Advanced Features (Future)

### **Phase 2 Enhancements:**

1. **Aging Report:**
   - Group payables by age (0-7 days, 8-14 days, 15-30 days, 30+ days)
   - Visualize as stacked bar chart

2. **Payment Schedule:**
   - Calendar view showing due dates
   - Color-coded by urgency
   - Drag-and-drop to reschedule

3. **Cash Flow Projection Graph:**
   - Line chart showing projected balance over next 90 days
   - Multiple scenarios (pessimistic, realistic, optimistic)
   - Based on recurring payables/receivables

4. **Automated Reminders:**
   - Desktop notifications for upcoming due dates
   - Email integration (optional)

5. **Payment Integration:**
   - Link to bank accounts
   - One-click payment (via API)
   - Auto-mark as paid when transaction detected

6. **Analytics:**
   - Average Days to Receive
   - Late Payment Rate
   - Creditor/Debtor reliability scoring

---

## ✅ Implementation Checklist

**Completed:**
- ✅ Data models (`Payable`, `Receivable`)
- ✅ DataService CRUD methods
- ✅ LiquidityViewModel with all commands
- ✅ PayableEditorDialog (XAML + code-behind)
- ✅ ReceivableEditorDialog (XAML + code-behind)
- ✅ LiquidityView.xaml (full UI)
- ✅ Navigation integration
- ✅ Monochrome Stealth Glass styling
- ✅ Forecast calculation logic
- ✅ Urgency/Probability color coding
- ✅ JSON persistence

**Testing Checklist:**
- [ ] Add payable → Verify forecast updates
- [ ] Mark payable as paid → Verify removed from list
- [ ] Add receivable with project link → Verify project dropdown
- [ ] Change probability → Verify weighted amount recalculates
- [ ] Delete operations → Confirm prompts
- [ ] Projected balance negative → Verify red warning
- [ ] Overdue payables → Verify critical alerts

---

## 📖 User Guide

### **Getting Started:**

1. Navigate to **"💧 Liquidity"** in sidebar
2. View **Liquidity Forecast** at top
3. Add obligations and receivables using **"➕ Add"** buttons

### **Managing Payables:**

**To add a bill/debt:**
1. Click "➕ Add" in OBLIGATIONS panel
2. Fill in title (e.g., "Server Invoice")
3. Enter creditor name
4. Set amount and due date
5. Click "✅ Save"

**When paid:**
1. Click "✅ Mark Paid" on the card
2. Entry moves to history (filtered out)

### **Managing Receivables:**

**To track incoming money:**
1. Click "➕ Add" in RECEIVABLES panel
2. Fill in title (e.g., "Freelance Payment")
3. Enter debtor name
4. Optionally link to project
5. Select probability level:
   - **Confirmed:** Guaranteed money (signed contract)
   - **Likely:** Expected (verbal agreement)
   - **Uncertain:** Speculative (proposal sent)
6. Set expected receive date
7. Click "✅ Save"

**When received:**
1. Click "✅ Mark Received"
2. Entry archived

### **Interpreting the Forecast:**

**Formula breakdown:**
- **Current Cash:** From all accounts in "Wallet" tab
- **Pending Receivables:** Sum of weighted receivables (not yet received)
- **Pending Payables:** Sum of unpaid obligations
- **Projected Balance:** What you'll have after all transactions settle

**Warning signs:**
- **Red Projected Balance:** Liquidity crisis - need to take action
- **Overdue badge:** Past-due payments, urgent attention needed
- **Critical badge:** Payments due within 3-7 days

---

## 🎯 Business Value

**For Freelancers:**
- Track client invoices and payment status
- Ensure you can cover expenses before they're due
- Identify slow-paying clients

**For Small Business Owners:**
- Manage vendor payments and supplier invoices
- Forecast cash runway
- Plan for seasonal cash flow variations

**For Developers/Solopreneurs:**
- Track project milestone payments
- Monitor SaaS/infrastructure bills
- Prevent service disruptions due to non-payment

---

## 💡 Best Practices

1. **Update Regularly:** Mark items as paid/received promptly for accurate forecasts
2. **Be Conservative:** Use "Likely" or "Uncertain" for speculative receivables
3. **Set Reminders:** Check Liquidity Manager daily if projected balance is tight
4. **Link to Projects:** Associate receivables with projects for better analytics
5. **Archive History:** Don't delete paid items - keep for historical analysis

---

## 🚀 **Liquidity Manager is Live!**

**Open the app and navigate to "💧 Liquidity" to start tracking your obligations and receivables!**

**Next:** Team & Payroll Management module for detailed project labor cost tracking.

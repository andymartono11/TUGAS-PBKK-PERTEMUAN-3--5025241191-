# TUGAS-PBKK-PERTEMUAN-3--5025241191-
Scientific Calculator dengan tema Glassmorphism + fitur History, dibuat dengan .NET WinForms.

# **Nama : Antonius Andy Martono**
# **Kelas : PBKK C**
# **NRP : 5025241191**

---

# GlassCalculator — Scientific Calculator (Glassmorphism Edition)

Kalkulator saintifik berbasis **.NET WinForms** dengan tema **Glassmorphism dark**, fitur **history perhitungan**, dan **recursive descent parser** buatan sendiri. Dibuat sebagai tugas PBKK C — berbeda dari contoh referensi dalam hal desain UI, arsitektur parser, dan fitur tambahan.

---

## Struktur File

```
GlassCalculator/
├── GlassCalculator.csproj     # Konfigurasi project .NET
├── Program.cs                 # Entry point aplikasi
├── ColorPalette.cs            # Definisi warna tema Glassmorphism
├── CalculatorEngine.cs        # Parser ekspresi matematika + history
├── MainForm.Designer.cs       # Seluruh kode UI (dibuat manual)
└── MainForm.cs                # Logika aplikasi & event handler
```

---

## 1. `GlassCalculator.csproj`

File konfigurasi project .NET yang mendefinisikan seluruh properti build.

```xml
<OutputType>WinExe</OutputType>
```
Output berupa aplikasi Windows GUI. Tidak membuka jendela console saat dijalankan.

```xml
<TargetFramework>net8.0-windows</TargetFramework>
```
Target .NET 8 khusus Windows. Suffix `-windows` wajib ada karena menggunakan WinForms.

```xml
<Nullable>disable</Nullable>
```
Nullable reference types dinonaktifkan agar kode lebih ringkas.

```xml
<UseWindowsForms>true</UseWindowsForms>
```
Mengaktifkan dukungan Windows Forms.

```xml
<ImplicitUsings>disable</ImplicitUsings>
```
Implicit using dimatikan — setiap file harus menulis `using` sendiri secara eksplisit agar dependensi terlihat jelas.

```xml
<RootNamespace>GlassCalculator</RootNamespace>
<AssemblyName>GlassCalculator</AssemblyName>
```
Namespace root dan nama file output `.exe` adalah `GlassCalculator`.

```xml
<ApplicationDefaultFont>Segoe UI, 10pt</ApplicationDefaultFont>
```
Font default seluruh aplikasi, 10pt untuk keterbacaan lebih baik pada tema gelap.

---

## 2. `Program.cs`

Entry point aplikasi.

```csharp
[STAThread]
static void Main()
{
    try
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.ToString(), "Startup Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

- **`[STAThread]`** — wajib ada di WinForms. Thread utama berjalan dalam mode Single-Threaded Apartment, diperlukan untuk interaksi COM (clipboard, drag-drop, dialog file).
- **`Application.SetHighDpiMode`** — membuat tampilan tajam di layar HiDPI/Retina.
- **`Application.EnableVisualStyles()`** — mengaktifkan gaya visual modern Windows.
- **`try/catch` di Main** — menangkap crash saat startup dan menampilkan pesan error via MessageBox, sehingga lebih mudah di-debug dibanding langsung hilang diam-diam.
- **`Application.Run(new MainForm())`** — menjalankan message loop Windows dengan `MainForm` sebagai form utama.

---

## 3. `ColorPalette.cs`

Memusatkan semua definisi warna agar konsisten di seluruh UI.

```csharp
public static class ColorPalette
{
    // Background
    public static readonly Color GradientTop    = ColorTranslator.FromHtml("#1a1a2e");
    public static readonly Color GradientBottom = ColorTranslator.FromHtml("#16213e");

    // Panel glass
    public static Color GlassFill(int alpha = 40)
        => Color.FromArgb(alpha, 255, 255, 255);
    public static Color GlassBorder(int alpha = 80)
        => Color.FromArgb(alpha, 255, 255, 255);

    // Tombol aksen
    public static readonly Color AccentBlue   = ColorTranslator.FromHtml("#4fc3f7");
    public static readonly Color AccentPurple = ColorTranslator.FromHtml("#ce93d8");
    public static readonly Color AccentGreen  = ColorTranslator.FromHtml("#a5d6a7");
    public static readonly Color AccentOrange = ColorTranslator.FromHtml("#ffcc80");
    public static readonly Color AccentRed    = ColorTranslator.FromHtml("#ef9a9a");
    public static readonly Color AccentCyan   = ColorTranslator.FromHtml("#80deea");

    // Teks
    public static readonly Color TextPrimary   = Color.White;
    public static readonly Color TextSecondary = Color.FromArgb(180, 255, 255, 255);
    public static readonly Color TextDim       = Color.FromArgb(100, 255, 255, 255);
}
```

Pembagian warna aksen per kelompok tombol:
| Warna | Tombol |
|---|---|
| **AccentBlue** | Fungsi saintifik (sin, cos, sqrt, dll) |
| **AccentPurple** | Memori (MC, MR, M+, M-, MS) |
| **AccentGreen** | Angka dan desimal |
| **AccentOrange** | Operator (+, -, ×, ÷, ^) |
| **AccentRed** | AC dan backspace |
| **AccentCyan** | Tombol = dan DEG/RAD |

---

## 4. `CalculatorEngine.cs`

Otak kalkulator. Berisi **recursive descent parser** yang ditulis manual tanpa library, dengan dukungan **history perhitungan**.

### 4.1 Kelas `CalculatorEngine`

```csharp
public List<string> History { get; } = new List<string>();

public double Evaluate(string expression, bool degreeMode, double ans)
```

Parameter:
- `expression` — string ekspresi matematika, misal `"sin(30)+2^3"`
- `degreeMode` — `true` = derajat, `false` = radian
- `ans` — nilai jawaban terakhir, untuk tombol `Ans`

**Normalisasi simbol:**
```csharp
expression = expression
    .Replace("×", "*").Replace("÷", "/")
    .Replace("−", "-").Replace("π", "pi")
    .Replace("√", "sqrt").Replace("%", "/100");
```
Karakter Unicode diganti ke ASCII. Tambahan vs referensi: `%` langsung dikonversi ke `/100` sehingga parser tidak perlu kasus khusus.

**Simpan ke history:**
```csharp
History.Add($"{originalExpression} = {FormatResult(result)}");
if (History.Count > 50) History.RemoveAt(0);
```
Maksimum 50 entri. Jika melebihi, entri tertua dihapus otomatis.

### 4.2 Kelas `Parser` (nested private)

Parser menggunakan pola **Recursive Descent**. Tiap level menangani satu kelompok operator dengan prioritas berbeda:

```
ParseExpression()    ← +, -  (prioritas terendah)
  ParseTerm()        ← *, /
    ParsePower()     ← ^     (right-associative)
      ParseUnary()   ← unary +/-
        ParsePostfix() ← !  (faktorial)
          ParsePrimary() ← angka, kurung, fungsi, konstanta
```

#### `ParseExpression()` vs `ParseExpressionInner()`

Perbedaan penting dari contoh referensi: parser ini memiliki **dua** versi parse level tertinggi.

```csharp
// Top-level: verifikasi seluruh string sudah habis
public double ParseExpression()
{
    double value = ParseExpressionInner();
    SkipSpaces();
    if (pos < text.Length)
        throw new Exception($"Karakter tidak dikenal: '{text[pos]}'");
    return value;
}

// Inner: dipakai di dalam kurung — tidak cek sisa string
private double ParseExpressionInner()
{
    double value = ParseTerm();
    while (true)
    {
        SkipSpaces();
        if (Match('+')) value += ParseTerm();
        else if (Match('-')) value -= ParseTerm();
        else break;
    }
    return value;
}
```

Ini **memperbaiki bug** dari contoh referensi: ekspresi `(2+3)*4` pada contoh gagal karena isi kurung di-parse dengan `ParseTerm()` sehingga operator `+` di dalam kurung tidak tertangani. Versi ini menggunakan `ParseExpressionInner()` di dalam kurung.

#### `ParseTerm()` — operator * dan /

```csharp
double value = ParsePower();
while (true)
{
    SkipSpaces();
    if (Match('*'))
        value *= ParsePower();
    else if (Match('/'))
    {
        double d = ParsePower();
        if (Math.Abs(d) < 1e-15)
            throw new DivideByZeroException();
        value /= d;
    }
    else break;
}
return value;
```

Cek pembagian nol menggunakan toleransi `1e-15` karena floating point tidak pernah persis 0.

#### `ParsePower()` — operator ^ (right-associative) + implicit multiplication

```csharp
double value = ParseUnary();
SkipSpaces();
if (Match('^'))
{
    double exp = ParsePower(); // rekursif = right-associative
    value = Math.Pow(value, exp);
}
// Implicit multiplication: 2pi, 3sin(30), (2+3)(4+1)
SkipSpaces();
if (pos < text.Length)
{
    char next = text[pos];
    if (char.IsLetter(next) || next == '(')
        value *= ParseUnary();
}
return value;
```

- Rekursi pada `ParsePower()` sendiri menjadikan `^` bersifat **right-associative**: `2^3^2` = `2^(3^2)` = 512, bukan `(2^3)^2` = 64.
- **Implicit multiplication** adalah fitur tambahan: `2pi`, `3sin(30)`, atau `(2+3)(4+1)` otomatis dianggap perkalian tanpa harus mengetik `*`.

#### Fungsi yang Didukung

Selain fungsi dasar, kalkulator ini menambahkan:

| Fungsi | Keterangan |
|---|---|
| `log2(x)` | Logaritma basis 2 |
| `sinh(x)`, `cosh(x)`, `tanh(x)` | Fungsi hiperbolik |
| `floor(x)`, `ceil(x)`, `round(x)` | Pembulatan |
| `max(a,b)`, `min(a,b)` | Dua argumen, dipisahkan koma |
| `rand()` | Bilangan acak 0–1 |

**Fungsi dua argumen (max, min):**
```csharp
case "max":
case "min": {
    double a = ParseExpressionInner();
    if (!Match(',')) throw new Exception($"{name} butuh dua argumen.");
    double b = ParseExpressionInner();
    if (!Match(')')) throw new Exception("Kurung tidak ditutup.");
    return name == "max" ? Math.Max(a, b) : Math.Min(a, b);
}
```

#### `ToRadians` dan `FromRadians`

```csharp
private double ToRadians(double v)
    => degreeMode ? v * Math.PI / 180.0 : v;

private double FromRadians(double v)
    => degreeMode ? v * 180.0 / Math.PI : v;
```

#### `Factorial(double value)`

```csharp
if (value < 0 || Math.Abs(value - Math.Round(value)) > 1e-10)
    throw new Exception("n! membutuhkan bilangan bulat >= 0.");
if (value > 170)
    throw new Exception("Angka terlalu besar untuk faktorial.");
double result = 1;
for (int i = 2; i <= (int)value; i++)
    result *= i;
return result;
```

170! adalah batas atas `double` (~7.26e306). Di atas itu hasilnya Infinity.

#### `FormatNumber(double value)`

```csharp
public static string FormatNumber(double value)
{
    if (Math.Abs(value) < 1e-12) value = 0;
    if (value != 0 && (Math.Abs(value) >= 1e15 || Math.Abs(value) < 1e-6))
        return value.ToString("E6", CultureInfo.InvariantCulture);
    return value.ToString("G14", CultureInfo.InvariantCulture);
}
```

- Angka di bawah `1e-12` dibulatkan ke 0 (menghindari `2.3e-17` yang seharusnya 0).
- Angka sangat besar atau sangat kecil otomatis tampil dalam notasi ilmiah.
- `G14` = 14 digit signifikan.
- `InvariantCulture` memastikan titik selalu jadi desimal (tidak terpengaruh setting regional Windows).

---

## 5. `MainForm.Designer.cs`

Membangun seluruh UI secara programatik. Tidak menggunakan Visual Studio Designer.

### 5.1 Tata Letak Keseluruhan

```
Form
├── headerPanel (DockStyle.Top, tinggi 78px)
│   ├── lblLogo, lblTitle, lblSubtitle
│   ├── btnDegRad (kanan atas)
│   └── btnToggleHistory (kanan atas)
│
└── root (TableLayoutPanel, 2 kolom, DockStyle.Fill)
    ├── Kolom 0 (68%) : left (TableLayoutPanel, 5 baris)
    │   ├── Baris 0 (150px) : displayPanel
    │   ├── Baris 1 (138px) : sciPanel + sciTable (5×3)
    │   ├── Baris 2 (50px)  : trigPanel + trigTable (3×1)
    │   ├── Baris 3 (50px)  : memPanel + memTable (5×1)
    │   └── Baris 4 (sisa)  : keypadPanel + keyTable (5×6)
    │
    └── Kolom 1 (32%) : historyPanel
        ├── lblHistoryTitle
        ├── historyList (ListBox)
        └── btnClearHistory
```

Pembagian dua kolom ini adalah **pembeda utama** dari contoh referensi yang hanya satu kolom.

### 5.2 Display Panel

Menampilkan tiga lapisan informasi:
- `lblHistoryLine` — ekspresi sebelumnya (misal `sin(30)+2^3 =`), warna abu-abu redup
- `lblExpression` — ekspresi yang sedang diketik, font 13pt, rata kanan
- `lblResult` — hasil perhitungan, font 30pt bold, warna putih, rata kanan

Urutan `Controls.Add` penting di WinForms Dock:
```csharp
displayPanel.Controls.Add(lblResult);       // Fill — ditambah pertama
displayPanel.Controls.Add(lblExpression);   // Top
displayPanel.Controls.Add(lblHistoryLine);  // Top
displayPanel.Controls.Add(lblMode);         // posisi bebas
displayPanel.Controls.Add(lblMemoryValue);  // posisi bebas
```
Kontrol yang di-add lebih awal berada di "belakang" secara Z-order, sehingga `lblResult` (Fill) tidak menutupi yang lain.

### 5.3 Helper `MakeBtn`

```csharp
private Button MakeBtn(string text, Color back, Color fore)
{
    var btn       = new Button();
    btn.Text      = text;
    btn.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
    btn.ForeColor = fore;
    btn.BackColor = back;
    btn.FlatStyle = FlatStyle.Flat;
    btn.FlatAppearance.BorderSize  = 1;
    btn.FlatAppearance.BorderColor = Color.FromArgb(60, fore);
    btn.Cursor    = Cursors.Hand;
    btn.Dock      = DockStyle.Fill;
    btn.Margin    = new Padding(3);
    return btn;
}
```

Semua tombol dibuat konsisten melalui helper ini. `BorderColor` menggunakan `Color.FromArgb(60, fore)` — border warna aksen dengan opacity 60, memberi kesan outline tipis yang menyatu dengan tema gelap.

### 5.4 Keypad Layout

Grid 5×6:
| Baris | Isi |
|---|---|
| 0 | `(` `)` `Ans` `÷` `⌫` |
| 1 | `7` `8` `9` `×` `EXP` |
| 2 | `4` `5` `6` `−` `RND` |
| 3 | `1` `2` `3` `+` `%` |
| 4 | `±` `0` `.` `^` `M` |
| 5 | `AC` (1 kolom) + `=` (4 kolom, `SetColumnSpan`) |

---

## 6. `MainForm.cs`

Logika aplikasi. Mengelola state, event handler, dan interaksi antara UI dengan `CalculatorEngine`.

### 6.1 Field State

```csharp
private readonly CalculatorEngine engine = new CalculatorEngine();
private double lastAnswer     = 0;      // nilai Ans terakhir
private double memory         = 0;      // nilai memori kalkulator
private bool   degreeMode     = true;   // true=DEG, false=RAD
private bool   secondMode     = false;  // mode 2nd (inverse trig)
private bool   justCalculated = false;  // true setelah tekan =
private bool   historyVisible = true;   // status panel history
```

### 6.2 `Calculate()`

```csharp
private void Calculate()
{
    string expr = txtExpression.Text.Trim();
    if (string.IsNullOrWhiteSpace(expr)) return;

    try
    {
        double result   = engine.Evaluate(expr, degreeMode, lastAnswer);
        lastAnswer      = result;
        lblHistoryLine.Text = expr + " =";
        lblResult.Text  = FormatNum(result);
        justCalculated  = true;
        UpdateHistoryList();
    }
    catch (DivideByZeroException)
    {
        lblResult.Text = "Tidak bisa ÷ 0";
        justCalculated = true;
    }
    catch (Exception ex)
    {
        lblResult.Text = ex.Message;
        justCalculated = true;
    }
}
```

Setiap kalkulasi yang berhasil memanggil `UpdateHistoryList()` untuk memperbarui panel history.

### 6.3 `justCalculated` flag

Flag ini mengontrol perilaku "smart clear":

```csharp
private void Number_Click(object sender, EventArgs e)
{
    if (justCalculated)
    {
        txtExpression.Clear();
        lblResult.Text = "0";
        justCalculated = false;
    }
    txtExpression.AppendText(btn.Text);
    UpdateDisplay();
}
```

Setelah `=` ditekan, jika user langsung mengetik angka baru, ekspresi lama otomatis dibersihkan. Tapi jika user menekan operator, ekspresi lama dipertahankan sehingga bisa melanjutkan dari hasil sebelumnya.

### 6.4 Toggle History Panel

```csharp
private void ToggleHistory_Click(object sender, EventArgs e)
{
    historyVisible = !historyVisible;
    historyPanel.Visible = historyVisible;

    var root = historyPanel.Parent as TableLayoutPanel;
    if (root != null)
    {
        root.ColumnStyles[1].Width = historyVisible ? 32f : 0f;
        root.ColumnStyles[0].Width = historyVisible ? 68f : 100f;
    }
    btnToggleHistory.Text = historyVisible ? "⊟ Hist" : "⊞ Hist";
}
```

Mengubah lebar kolom `TableLayoutPanel` secara dinamis. Saat history disembunyikan, kolom kiri mengambil 100% lebar — berguna untuk layar kecil atau saat video demo.

### 6.5 Double-click History untuk Load Ulang

```csharp
private void HistoryList_DoubleClick(object sender, EventArgs e)
{
    if (historyList.SelectedItem is string entry)
    {
        int idx = entry.LastIndexOf(" = ");
        if (idx >= 0)
            txtExpression.Text = entry.Substring(0, idx);
        justCalculated = false;
        UpdateDisplay();
    }
}
```

Double-click pada entri history memuat kembali **ekspresi** (bukan hasilnya) ke input. User bisa langsung mengedit dan menghitung ulang.

### 6.6 Second Mode (tombol 2nd)

```csharp
private void Second_Click(object sender, EventArgs e)
{
    secondMode = !secondMode;
    if (secondMode)
    {
        btnSin.Text = "sin⁻¹"; btnSin.Tag = "asin";
        btnCos.Text = "cos⁻¹"; btnCos.Tag = "acos";
        btnTan.Text = "tan⁻¹"; btnTan.Tag = "atan";
        // highlight tombol 2nd
        foreach (Control c in sciTable.Controls)
            if (c is Button b && (string)b.Tag == "2nd")
                b.BackColor = Color.FromArgb(80, ColorPalette.AccentCyan);
    }
    else
    {
        btnSin.Text = "sin"; btnSin.Tag = "sin";
        btnCos.Text = "cos"; btnCos.Tag = "cos";
        btnTan.Text = "tan"; btnTan.Tag = "tan";
        // kembalikan warna
        foreach (Control c in sciTable.Controls)
            if (c is Button b && (string)b.Tag == "2nd")
                b.BackColor = Color.FromArgb(25, 45, 80);
    }
}
```

Saat mode 2nd aktif, label tombol trig berubah ke fungsi invers dan tombol 2nd berubah warna ke cyan terang.

### 6.7 DEG/RAD Toggle

```csharp
private void UpdateModeUI()
{
    string mode    = degreeMode ? "DEG" : "RAD";
    btnDegRad.Text = mode;
    lblMode.Text   = mode;
    lblMode.ForeColor = degreeMode
        ? Color.FromArgb(79, 195, 247)   // biru muda = DEG
        : Color.FromArgb(255, 200, 100); // oranye = RAD
}
```

Mode DEG dan RAD dibedakan dengan warna label yang berbeda agar mudah dilihat sekilas.

### 6.8 Memory

```csharp
private void Memory_Click(object sender, EventArgs e)
{
    string cmd = btn.Tag?.ToString() ?? "";
    switch (cmd)
    {
        case "MC": memory = 0; break;
        case "MR": txtExpression.AppendText(FormatNum(memory)); break;
        case "M+": memory += GetCurrentValue(); break;
        case "M-": memory -= GetCurrentValue(); break;
        case "MS": memory  = GetCurrentValue(); break;
    }
    UpdateMemoryDisplay();
}

private void UpdateMemoryDisplay()
{
    lblMemoryValue.Text = memory != 0 ? "M: " + FormatNum(memory) : "";
}
```

Nilai memori saat ini ditampilkan di label kecil di atas display. Kalau memori = 0, label kosong.

### 6.9 Keyboard Shortcut

```csharp
protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    switch (keyData)
    {
        case Keys.D0: case Keys.NumPad0: Append("0"); return true;
        // ... D1-D9, NumPad1-9 ...
        case Keys.Enter:   Calculate(); return true;
        case Keys.Back:    Backspace_Click(null, null); return true;
        case Keys.Escape:  Clear_Click(null, null); return true;
        case Keys.H:       ToggleHistory_Click(null, null); return true;
        case Keys.Add:     Append("+"); return true;
        case Keys.Subtract: Append("-"); return true;
        case Keys.Multiply: Append("*"); return true;
        case Keys.Divide:   Append("/"); return true;
    }
    return base.ProcessCmdKey(ref msg, keyData);
}
```

Tambahan shortcut `H` untuk toggle panel history yang tidak ada di contoh referensi.

---


## VIDEO
https://drive.google.com/file/d/1-1VnT8qC9No-XWvndZOxbFC7KImgb3dc/view?usp=sharing

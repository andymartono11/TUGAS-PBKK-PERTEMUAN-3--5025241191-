using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace GlassCalculator
{
    public partial class MainForm : Form
    {
        private readonly CalculatorEngine engine = new CalculatorEngine();
        private double lastAnswer    = 0;
        private double memory        = 0;
        private bool   degreeMode    = true;
        private bool   secondMode    = false;
        private bool   justCalculated = false;
        private bool   historyVisible = true;

        public MainForm()
        {
            InitializeComponent();
            UpdateModeUI();
            UpdateDisplay();
        }

        // ── Display ───────────────────────────────────────────────────────────
        private void UpdateDisplay()
        {
            lblExpression.Text = txtExpression.Text;
        }

        private string FormatNum(double v) => CalculatorEngine.FormatNumber(v);

        // ── Number ────────────────────────────────────────────────────────────
        private void Number_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (justCalculated)
            {
                txtExpression.Clear();
                lblResult.Text  = "0";
                justCalculated  = false;
            }
            txtExpression.AppendText(btn.Text);
            UpdateDisplay();
        }

        // ── Operator ──────────────────────────────────────────────────────────
        private void Operator_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            justCalculated = false;
            string op = btn.Text;
            if (op == "×") op = "*";
            if (op == "÷") op = "/";
            if (op == "−") op = "-";
            txtExpression.AppendText(op);
            UpdateDisplay();
        }

        // ── Parenthesis ───────────────────────────────────────────────────────
        private void Parenthesis_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (justCalculated) { txtExpression.Clear(); justCalculated = false; }
            txtExpression.AppendText(btn.Text);
            UpdateDisplay();
        }

        // ── Decimal ───────────────────────────────────────────────────────────
        private void Decimal_Click(object sender, EventArgs e)
        {
            if (justCalculated) { txtExpression.Clear(); justCalculated = false; }
            txtExpression.AppendText(".");
            UpdateDisplay();
        }

        // ── Equals ────────────────────────────────────────────────────────────
        private void Equals_Click(object sender, EventArgs e) => Calculate();

        private void Calculate()
        {
            string expr = txtExpression.Text.Trim();
            if (string.IsNullOrWhiteSpace(expr)) return;

            try
            {
                double result  = engine.Evaluate(expr, degreeMode, lastAnswer);
                lastAnswer     = result;
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

        // ── Clear ─────────────────────────────────────────────────────────────
        private void Clear_Click(object sender, EventArgs e)
        {
            txtExpression.Clear();
            lblExpression.Text = "";
            lblHistoryLine.Text    = "";
            lblResult.Text     = "0";
            justCalculated     = false;
        }

        // ── Backspace ─────────────────────────────────────────────────────────
        private void Backspace_Click(object sender, EventArgs e)
        {
            if (txtExpression.Text.Length == 0) return;
            txtExpression.Text = txtExpression.Text.Substring(0, txtExpression.Text.Length - 1);
            justCalculated = false;
            UpdateDisplay();
        }

        // ── Sign ─────────────────────────────────────────────────────────────
        private void Sign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExpression.Text)) return;
            txtExpression.Text = "-(" + txtExpression.Text + ")";
            justCalculated = false;
            UpdateDisplay();
        }

        // ── Percent ───────────────────────────────────────────────────────────
        private void Percent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExpression.Text)) return;
            txtExpression.Text = "(" + txtExpression.Text + ")/100";
            justCalculated = false;
            UpdateDisplay();
        }

        // ── Power ─────────────────────────────────────────────────────────────
        private void Power_Click(object sender, EventArgs e)
        {
            txtExpression.AppendText("^");
            justCalculated = false;
            UpdateDisplay();
        }

        // ── EXP (scientific notation entry) ──────────────────────────────────
        private void Exp_Click(object sender, EventArgs e)
        {
            txtExpression.AppendText("e");
            justCalculated = false;
            UpdateDisplay();
        }

        // ── RND ───────────────────────────────────────────────────────────────
        private void Rnd_Click(object sender, EventArgs e)
        {
            double v = GetCurrentValue();
            txtExpression.Text = Math.Round(v).ToString(CultureInfo.InvariantCulture);
            justCalculated = false;
            UpdateDisplay();
        }

        // ── Ans ───────────────────────────────────────────────────────────────
        private void Ans_Click(object sender, EventArgs e)
        {
            txtExpression.AppendText("ans");
            justCalculated = false;
            UpdateDisplay();
        }

        // ── Scientific buttons ────────────────────────────────────────────────
        private void Scientific_Click(object sender, EventArgs e)
        {
            var btn      = (Button)sender;
            string func  = btn.Tag?.ToString() ?? "";
            if (func == "") return;

            switch (func)
            {
                case "pi":
                    txtExpression.AppendText("pi"); break;
                case "e":
                    txtExpression.AppendText("e"); break;
                case "factorial":
                    txtExpression.AppendText("!"); break;
                case "pow":
                    txtExpression.AppendText("^"); break;
                case "sq":
                    if (!string.IsNullOrWhiteSpace(txtExpression.Text))
                        txtExpression.Text = "sq(" + txtExpression.Text + ")";
                    else txtExpression.AppendText("sq("); break;
                case "cube":
                    if (!string.IsNullOrWhiteSpace(txtExpression.Text))
                        txtExpression.Text = "cube(" + txtExpression.Text + ")";
                    else txtExpression.AppendText("cube("); break;
                case "inv":
                    if (!string.IsNullOrWhiteSpace(txtExpression.Text))
                        txtExpression.Text = "inv(" + txtExpression.Text + ")";
                    else txtExpression.AppendText("inv("); break;
                case "sqrt":
                    if (!string.IsNullOrWhiteSpace(txtExpression.Text))
                        txtExpression.Text = "sqrt(" + txtExpression.Text + ")";
                    else txtExpression.AppendText("sqrt("); break;
                case "cbrt":
                    if (!string.IsNullOrWhiteSpace(txtExpression.Text))
                        txtExpression.Text = "cbrt(" + txtExpression.Text + ")";
                    else txtExpression.AppendText("cbrt("); break;
                default:
                    txtExpression.AppendText(func + "("); break;
            }

            justCalculated = false;
            UpdateDisplay();
        }

        // ── Trig buttons ──────────────────────────────────────────────────────
        private void Trig_Click(object sender, EventArgs e)
        {
            var btn     = (Button)sender;
            string func = btn.Tag?.ToString() ?? "";
            if (func == "") return;
            txtExpression.AppendText(func + "(");
            justCalculated = false;
            UpdateDisplay();
        }

        // ── 2nd mode ──────────────────────────────────────────────────────────
        private void Second_Click(object sender, EventArgs e)
        {
            secondMode = !secondMode;
            if (secondMode)
            {
                btnSin.Text = "sin⁻¹"; btnSin.Tag = "asin";
                btnCos.Text = "cos⁻¹"; btnCos.Tag = "acos";
                btnTan.Text = "tan⁻¹"; btnTan.Tag = "atan";

                // Find "2nd" button and highlight it
                foreach (Control c in sciTable.Controls)
                    if (c is Button b && (string)b.Tag == "2nd")
                        b.BackColor = Color.FromArgb(80, ColorPalette.AccentCyan);
            }
            else
            {
                btnSin.Text = "sin"; btnSin.Tag = "sin";
                btnCos.Text = "cos"; btnCos.Tag = "cos";
                btnTan.Text = "tan"; btnTan.Tag = "tan";

                foreach (Control c in sciTable.Controls)
                    if (c is Button b && (string)b.Tag == "2nd")
                        b.BackColor = Color.FromArgb(30, ColorPalette.AccentBlue);
            }
        }

        // ── DEG/RAD ───────────────────────────────────────────────────────────
        private void DegRad_Click(object sender, EventArgs e)
        {
            degreeMode = !degreeMode;
            UpdateModeUI();
        }

        private void UpdateModeUI()
        {
            string mode       = degreeMode ? "DEG" : "RAD";
            btnDegRad.Text    = mode;
            lblMode.Text      = mode;
            lblMode.ForeColor = degreeMode
                ? ColorPalette.AccentCyan
                : Color.FromArgb(255, 200, 100);
        }

        // ── Memory ────────────────────────────────────────────────────────────
        private void Memory_Click(object sender, EventArgs e)
        {
            var btn     = (Button)sender;
            string cmd  = btn.Tag?.ToString() ?? "";
            switch (cmd)
            {
                case "MC": memory = 0; break;
                case "MR":
                    txtExpression.AppendText(FormatNum(memory));
                    justCalculated = false;
                    UpdateDisplay();
                    break;
                case "M+": memory += GetCurrentValue(); break;
                case "M-": memory -= GetCurrentValue(); break;
                case "MS": memory  = GetCurrentValue(); break;
            }
            UpdateMemoryDisplay();
        }

        private void MemRecall_Click(object sender, EventArgs e)
        {
            txtExpression.AppendText(FormatNum(memory));
            justCalculated = false;
            UpdateDisplay();
        }

        private void UpdateMemoryDisplay()
        {
            lblMemoryValue.Text = memory != 0
                ? "M: " + FormatNum(memory)
                : "";
        }

        private double GetCurrentValue()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtExpression.Text)) return 0;
                return engine.Evaluate(txtExpression.Text, degreeMode, lastAnswer);
            }
            catch { return 0; }
        }

        // ── History ───────────────────────────────────────────────────────────
        private void UpdateHistoryList()
        {
            historyList.Items.Clear();
            for (int i = engine.History.Count - 1; i >= 0; i--)
                historyList.Items.Add(engine.History[i]);
        }

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

        private void ClearHistory_Click(object sender, EventArgs e)
        {
            engine.History.Clear();
            historyList.Items.Clear();
            lblHistoryLine.Text = "";
        }

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
            btnToggleHistory.Text = historyVisible ? "⊟ History" : "⊞ History";
        }

        // ── Keyboard ─────────────────────────────────────────────────────────
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            void Append(string s)
            {
                if (justCalculated) { txtExpression.Clear(); lblResult.Text = "0"; justCalculated = false; }
                txtExpression.AppendText(s);
                UpdateDisplay();
            }

            switch (keyData)
            {
                case Keys.D0: case Keys.NumPad0: Append("0"); return true;
                case Keys.D1: case Keys.NumPad1: Append("1"); return true;
                case Keys.D2: case Keys.NumPad2: Append("2"); return true;
                case Keys.D3: case Keys.NumPad3: Append("3"); return true;
                case Keys.D4: case Keys.NumPad4: Append("4"); return true;
                case Keys.D5: case Keys.NumPad5: Append("5"); return true;
                case Keys.D6: case Keys.NumPad6: Append("6"); return true;
                case Keys.D7: case Keys.NumPad7: Append("7"); return true;
                case Keys.D8: case Keys.NumPad8: Append("8"); return true;
                case Keys.D9: case Keys.NumPad9: Append("9"); return true;
                case Keys.OemPeriod:    Append("."); return true;
                case Keys.Add:          Append("+"); return true;
                case Keys.OemMinus:
                case Keys.Subtract:     Append("-"); return true;
                case Keys.Multiply:     Append("*"); return true;
                case Keys.Divide:       Append("/"); return true;
                case Keys.D5 | Keys.Shift: Append("%"); return true;
                case Keys.Enter:        Calculate(); return true;
                case Keys.Back:         Backspace_Click(null, null); return true;
                case Keys.Escape:       Clear_Click(null, null); return true;
                case Keys.H:            ToggleHistory_Click(null, null); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}

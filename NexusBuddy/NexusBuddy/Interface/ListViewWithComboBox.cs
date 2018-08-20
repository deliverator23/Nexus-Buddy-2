using Firaxis.Framework.Granny;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace NexusBuddy
{
	public class ListViewWithComboBox : ListView
	{
		private ListViewItem item;
		private string subItemText = "";
		private int selectedSubItem;
		private ComboBox comboBoxMaterials = new ComboBox();
		public void FillCombo()
		{
			this.comboBoxMaterials.Items.Clear();
			this.comboBoxMaterials.Items.Add("<unassigned>");
			foreach (IGrannyMaterial current in NexusBuddyApplicationForm.loadedFile.Materials)
			{
				this.comboBoxMaterials.Items.Add(current.Name);
			}
		}
		public ListViewWithComboBox()
		{
			this.comboBoxMaterials.Size = new Size(0, 0);
			this.comboBoxMaterials.Location = new Point(0, 0);
			base.Controls.AddRange(new Control[]
			{
				this.comboBoxMaterials
			});
			this.comboBoxMaterials.SelectedIndexChanged += new EventHandler(this.MaterialSelected);
            this.comboBoxMaterials.LostFocus += new EventHandler(this.MaterialFocusExit);
            this.comboBoxMaterials.KeyPress += new KeyPressEventHandler(this.MaterialKeyPress);
			this.comboBoxMaterials.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxMaterials.Hide();
			base.Size = new Size(0, 0);
			base.TabIndex = 0;
			base.View = View.Details;
			base.MouseDown += new MouseEventHandler(this.ListViewMouseDown);
			base.DoubleClick += new EventHandler(this.ListViewDoubleClick);
			base.Click += new EventHandler(this.ListViewDoubleClick);
			base.GridLines = true;
		}
		private void MaterialKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r' || e.KeyChar == '\u001b')
			{
				this.comboBoxMaterials.Hide();
				NexusBuddyApplicationForm.form.UpdateMaterialBinding(this.comboBoxMaterials.SelectedItem as string);
			}
		}
        private void MaterialSelected(object sender, EventArgs e)
		{
			int selectedIndex = this.comboBoxMaterials.SelectedIndex;
			if (selectedIndex >= 0)
			{
				string text = this.comboBoxMaterials.Items[selectedIndex].ToString();
				this.item.SubItems[this.selectedSubItem].Text = text;
				NexusBuddyApplicationForm.form.UpdateMaterialBinding(this.comboBoxMaterials.SelectedItem as string);
			}
		}
        private void MaterialFocusExit(object sender, EventArgs e)
		{
			this.comboBoxMaterials.Hide();
		}
		public void ListViewDoubleClick(object sender, EventArgs e)
		{
			int num = 0;
			int num2 = base.Columns[0].Width;
			for (int i = 0; i < base.Columns.Count; i++)
			{
				if (i == 1)
				{
					this.selectedSubItem = i;
					break;
				}
				num = num2;
				num2 += base.Columns[i].Width;
			}
			this.subItemText = this.item.SubItems[this.selectedSubItem].Text;
			new Rectangle(num, this.item.Bounds.Top, num2, this.item.Bounds.Bottom);
			this.comboBoxMaterials.Size = new Size(num2 - num, this.item.Bounds.Bottom - this.item.Bounds.Top);
			this.comboBoxMaterials.Location = new Point(num, this.item.Bounds.Y);
			this.FillCombo();
			this.comboBoxMaterials.Show();
			this.comboBoxMaterials.Text = this.subItemText;
			this.comboBoxMaterials.SelectAll();
			this.comboBoxMaterials.Focus();
		}
		public void ListViewMouseDown(object sender, MouseEventArgs e)
		{
			this.item = base.GetItemAt(e.X, e.Y);
		}
	}
}

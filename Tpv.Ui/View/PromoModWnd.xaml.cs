using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Tpv.Ui.Model;
using Tpv.Ui.Service;

namespace Tpv.Ui.View
{
    /// <summary>
    /// Interaction logic for PromoModWnd.xaml
    /// </summary>
    public partial class PromoModWnd
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PromoModWnd));

        public int TermId { get; set; }
        public int CheckId { get; set; }
        public int CodeGroupModifier { get; set; }
        public string NameGroupModifier { get; set; }
        public List<ItemGroupModifier> LstModifierGroup { get; set; }

        private StackPanel _selectedItemTicket;

        private ItemGroupModifier _selectedGrpModifer;

        private const int NORMAL_MOD = 1;

        private int _selectedModCode = NORMAL_MOD;

        public PromoModWnd()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitModifierGroup();
        }

        private void InitModifierGroup()
        {
            TxtTitle.Text = "Modify " + NameGroupModifier;
            SetItemList(new ItemTicket
            {
                Name = NameGroupModifier,
                IsMain = true,
                PriceVal = 0,
            });
            ReadFromAloha();
        }

        private void SetItemList(ItemTicket ticket)
        {            
            if (!ticket.IsMain)
            {
                ticket.ModCode = _selectedModCode;
                _selectedModCode = NORMAL_MOD;
 
                var lstItem = GetTicketItems().Where(e => e.ItemModifier.GroupModifier == _selectedGrpModifer).ToList();
                if (lstItem.Count >= _selectedGrpModifer.MaximumVal)
                {
                    var ticketOld = lstItem[lstItem.Count - 1];
                    UpdateLastItemTicket(ticket, ticketOld);
                }
                else
                {
                    InsertItemTicket(ticket);
                }
            }
            else
            {
                InsertItemTicket(ticket);
                return;
            }

            if (_selectedGrpModifer == null)
                _selectedGrpModifer = ticket.ItemModifier.GroupModifier;

            var bFound = false;
            for (int i = 0, len = StGroupMod.Children.Count; i < len; i++)
            {
                var btnGrpModifier = StGroupMod.Children[i] as Button;

                if (btnGrpModifier == null)
                    continue;

                var grpModifier = btnGrpModifier.Tag as ItemGroupModifier;

                if (grpModifier == null)
                    continue;

                if (bFound)
                {
                    btnGrpModifier.Focus();
                    FillModifierSection(grpModifier);
                    return;
                }

                if (_selectedGrpModifer == grpModifier)
                    bFound = true;
            }

            CheckAndCloseWithOk();
        }

        private void UpdateLastItemTicket(ItemTicket ticket, ItemTicket ticketOld)
        {

            var stPanelFull = StPnTicket.Children.OfType<StackPanel>().Select(e => new{
                item = e.Tag as ItemTicket,
                stPanel = e
            }).FirstOrDefault(e => e.item != null && e.item == ticketOld);

            if (stPanelFull == null)
                return;

            if (stPanelFull.stPanel.Children.Count < 2)
                return;

            var txtField = stPanelFull.stPanel.Children[0] as TextBlock;
            if (txtField == null)
                return;

            txtField.Text = ticket.NameFull;

            txtField = stPanelFull.stPanel.Children[1] as TextBlock;
            if (txtField == null)
                return;

            txtField.Text = ticket.PriceTxt;
            stPanelFull.stPanel.Tag = ticket;
        }

        private void InsertItemTicket(ItemTicket ticket)
        {
            var txtItem = new TextBlock
            {
                Text = ticket.NameFull,
                //Foreground = new SolidColorBrush(Color.FromRgb(51, 204, 255)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5, 0, 1, 0),
                FontFamily = new FontFamily("Batang"),
                FontSize = 16,
                Width = 220
            };

            var txtPrice = new TextBlock
            {
                Text = ticket.PriceTxt,
                //Foreground = new SolidColorBrush(Color.FromRgb(51, 204, 255)),
                HorizontalAlignment = HorizontalAlignment.Right,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(1, 0, 0, 0),
                FontFamily = new FontFamily("Batang"),
                FontSize = 16,
                Width = 50
            };

            var stPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                //Background = new SolidColorBrush(Colors.Black),
                Tag = ticket
            };

            stPanel.MouseUp += StPanelOnMouseUp;

            stPanel.Children.Add(txtItem);
            stPanel.Children.Add(txtPrice);

            StPnTicket.Children.Add(stPanel);
        }

        private void StPanelOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var stkPanel = sender as StackPanel;

            if (stkPanel == null)
                return;

            var item = stkPanel.Tag as ItemTicket;

            if (item == null)
                return;

            ClearAllItems();
            _selectedItemTicket = null;

            if (item.IsMain)
                return;

            _selectedItemTicket = stkPanel;
            stkPanel.Background = new SolidColorBrush(Colors.Black);
            foreach (var childInt in stkPanel.Children)
            {
                var txtChild = childInt as TextBlock;

                if (txtChild == null)
                    continue;

                txtChild.Foreground = new SolidColorBrush(Color.FromRgb(51, 204, 255));
            }
        }

        private void ClearAllItems()
        {
            foreach (var child in StPnTicket.Children)
            {
                var stChild = child as StackPanel;

                if (stChild == null)
                    continue;

                stChild.Background = null;
                foreach (var childInt in stChild.Children)
                {
                    var txtChild = childInt as TextBlock;

                    if (txtChild == null)
                        continue;

                    txtChild.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void ReadFromAloha()
        {
            try
            {
                LstModifierGroup = new List<ItemGroupModifier>();

                LasaFOHLib67.IberDepot depot;
                LasaFOHLib67.IIberObject parent;

                try
                {
                    try
                    {
                        depot = new LasaFOHLib67.IberDepotClass();
                        LasaFOHLib67.IIberObject localState = depot.GetEnum(720).First();

                        try
                        {
                            // get the current employee
                            LasaFOHLib67.IIberObject emp = localState.GetEnum(723).First();
                            Console.WriteLine("Empleado: {0} {1}", emp.GetStringVal("FIRSTNAME"), emp.GetStringVal("LASTNAME"));
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message + " | " + ex.StackTrace);
                            MessageBox.Show("Verifique que exista un usuario con sesión iniciada en Aloha.", "TPV error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Close();
                        }

                        TermId = localState.GetLongVal("TERMINAL_NUM");
                        CheckId = localState.GetLongVal("CURRENT_CHECK_ID");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " | " + ex.StackTrace);
                        MessageBox.Show("Verifique que exista un ticket activo.", "TPV error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Close();
                    }

                    depot = new LasaFOHLib67.IberDepotClass();
                    parent = depot.FindObjectFromId(740, CodeGroupModifier).First();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + " | " + ex.StackTrace);
                    MessageBox.Show("Aloha no se está ejecutando o no existe un ticket activo.", "TPV error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Close();
                    return;
                }

                var modgrpIds = new List<int>();
                for (var x = 1; x <= 10; x++)
                {
                    var id = parent.GetLongVal(string.Format("MOD{0}", x));
                    if (id > 0)
                        modgrpIds.Add(id);
                }

                try
                {
                    LasaFOHLib67.IIberEnum modgroups = depot.GetEnum(16);
                    LasaFOHLib67.IIberObject modgroup = modgroups.First();
                    while (modgroup != null)
                    {
                        var mgid = modgroup.GetLongVal("ID");
                        if (modgrpIds.Contains(mgid))
                        {
                            LstModifierGroup.Add(new ItemGroupModifier
                            {
                                Name = modgroup.GetStringVal("LONGNAME"),
                                Minimum = modgroup.GetStringVal("MINIMUM"),
                                Maximum = modgroup.GetStringVal("MAXIMUM"),
                                Id = mgid
                            });
                        }
                        modgroup = modgroups.Next();
                    }
                }
                catch (Exception)
                {
                    // expected
                }

            }
            catch (Exception)
            {
                // expected
            }


            if (LstModifierGroup.Count == 0)
                return;

            LstModifierGroup = LstModifierGroup.OrderByDescending(e => e.MaximumVal).ThenBy(e => e.Name).ToList();

            var i = 0;
            foreach (var groupModifier in LstModifierGroup)
            {
                var button = FactoryButton.CreateModifierGroupButton(groupModifier, i++, OnClickGroupModifier);
                StGroupMod.Children.Add(button);

                if (i == 1) //El primero debe de ir por sus modificadores
                    FillModifierSection(groupModifier);

            }
        }



        public void OnClickGroupModifier(object o, RoutedEventArgs routedEventArgs)
        {
            var button = o as Button;

            if (button == null)
                return;

            var item = button.Tag as ItemGroupModifier;

            //Validate if ticket must have at least the minium of group modifier
            if (_selectedGrpModifer != null)
            {
                var minSel = _selectedGrpModifer.MinimumVal;
                var lstTicket = GetTicketItems();
                if (lstTicket.Count(e => e.ItemModifier.GroupModifier == _selectedGrpModifer) < minSel)
                {
                    ShowMessage("You cannot skip required modifier groups the first time you modify an item.");
                    SelectGroupModifierButton(_selectedGrpModifer);
                    return;
                }
            }


            if (item != null)
                FillModifierSection(item);
        }

        private void SelectGroupModifierButton(ItemGroupModifier selectedGrpModifer)
        {
            var button = StGroupMod.Children.OfType<Button>().Select(e => new
            {
                item = e.Tag as ItemGroupModifier,
                button = e
            }).FirstOrDefault(e => e.item != null && e.item == selectedGrpModifer);

            if (button == null)
                return;

            button.button.Focus();
            _selectedGrpModifer = button.item;
        }

        public List<ItemTicket> GetTicketItems()
        {
            return
                StPnTicket.Children.OfType<StackPanel>()
                    .Select(item => item.Tag as ItemTicket)
                    .Where(itemTk => itemTk != null && itemTk.IsMain == false)
                    .ToList();
        }


        private void FillModifierSection(ItemGroupModifier groupModifier)
        {
            WrModifier.Children.Clear();

            try
            {
                LasaFOHLib67.IberDepot depot = new LasaFOHLib67.IberDepotClass();
                LasaFOHLib67.IIberObject modgroup = null;
                // get the groupModifier group
                try
                {
                    LasaFOHLib67.IIberEnum modgroups = depot.GetEnum(16);
                    modgroup = modgroups.First();
                    while (modgroup != null)
                    {
                        if (modgroup.GetLongVal("ID") == groupModifier.Id)
                            break;

                        modgroup = modgroups.Next();
                    }
                }
                catch (Exception)
                {
                    // expected
                }

                if (modgroup == null)
                {
                    // sanity check.
                    // we shouldn't have gotten here if the groupModifier group doesn't exist
                    return;
                }

                var listModifiers = new List<ItemModifier>();
                // there can be up to 54 modifiers in a group
                for (int x = 1; x <= 54; x++)
                {
                    int itemId = modgroup.GetLongVal(String.Format("ITEM{0:D2}", x));
                    if (itemId > 0)
                    {
                        // find the item
                        try
                        {
                            LasaFOHLib67.IIberObject item = depot.FindObjectFromId(740, itemId).First();
                            listModifiers.Add(new ItemModifier
                            {
                                Id = itemId,
                                Name = item.GetStringVal("LONGNAME"),
                                Price = item.GetStringVal("PRICE"),
                                GroupModifier = groupModifier
                            });
                        }
                        catch (Exception)
                        {
                            // expected
                        }
                    }
                }

                listModifiers = listModifiers.OrderBy(e => e.Name).ToList();
                foreach (var modifier in listModifiers)
                {
                    var button = FactoryButton.CreateModifierButton(modifier, OnClickModifier);
                    WrModifier.Children.Add(button);
                }

                _selectedGrpModifer = groupModifier;
                SetCurrentTxtChoose();
            }
            catch (Exception)
            {
                // expected
            }
        }

        private void SetCurrentTxtChoose()
        {
            TxtChoose.Text = _selectedGrpModifer.MinimumVal == 0 ? String.Format("Choose up to {0}", _selectedGrpModifer.MaximumVal) : String.Format("Choose {0}", _selectedGrpModifer.MinimumVal);
        }

        public void OnClickModifier(object o, RoutedEventArgs routedEventArgs)
        {
            var button = o as Button;

            if (button == null)
                return;

            var item = button.Tag as ItemModifier;

            if (item == null)
                return;

            SetItemList(new ItemTicket
            {
                ItemModifier = item,
                Name = item.Name,
                PriceVal = item.PriceVal,
                IsMain = false
            });
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CheckAndCloseWithOk();
        }

        private void CheckAndCloseWithOk()
        {
            var ticket = GetTicketItems();

            foreach (var groupModifier in LstModifierGroup)
            {
                if (groupModifier.MinimumVal <= 0)
                    continue;

                if (ticket.Count(i => i.ItemModifier.GroupModifier == groupModifier) >= groupModifier.MinimumVal)
                    continue;

                ShowMessage(String.Format("You must have at least {0} item(s) from the {1} group.", groupModifier.MinimumVal,
                    groupModifier.Name));
                return;
            }
            DialogResult = true;
            Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedItemTicket == null)
            {
                ShowMessage("Select the modifier to delete by touching the Work Area above this button.");
                return;
            }

            StPnTicket.Children.Remove(_selectedItemTicket);
            _selectedItemTicket = null;
        }

        private void ShowMessage(string msgTx)
        {
            var msg = new MsgWnd(msgTx)
            {
                Owner = this
            };
            msg.ShowDialog();
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            ScVwMod.PageDown();
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            ScVwMod.PageUp();
        }

        private void ScVwMod_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            BtnDown.Visibility = Visibility.Hidden;
            BtnUp.Visibility = Visibility.Hidden;

            if (ScVwMod.ExtentHeight > ScVwMod.ViewportHeight && ScVwMod.ContentVerticalOffset + ScVwMod.ViewportHeight < ScVwMod.ExtentHeight)
                BtnDown.Visibility = Visibility.Visible;

            if (ScVwMod.ContentVerticalOffset > 0)
                BtnUp.Visibility = Visibility.Visible;
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            for (var i = StPnTicket.Children.Count - 1; i >= 0; i--)
            {
                var item = StPnTicket.Children[i] as StackPanel;

                if (item == null)
                    continue;

                var itemTag = item.Tag as ItemTicket;

                if (itemTag == null || itemTag.ItemModifier == null)
                    continue;

                if (itemTag.ItemModifier.GroupModifier != _selectedGrpModifer)
                    continue;

                if (Equals(_selectedItemTicket, item))
                    _selectedItemTicket = null;

                StPnTicket.Children.RemoveAt(i);
            }
        }

        private void BtnModCode_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            int iVal;

            if (int.TryParse((string) button.Tag, out iVal) == false)
                return;

            _selectedModCode = iVal;

            ModCode code;
            if (ItemTicket.DicModCode.TryGetValue(_selectedModCode, out code))
                TxtChoose.Text = String.Format("Select \"{0}\" item", code.Name);
        }
    }
}

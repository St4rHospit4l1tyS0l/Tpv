using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tpv.Ui.Model;
using Tpv.Ui.Service;

namespace Tpv.Ui.View
{
    /// <summary>
    /// Interaction logic for PromoModWnd.xaml
    /// </summary>
    public partial class PromoModWnd
    {

        public PromoModWnd()
        {
            InitializeComponent();
        }

        public int CodeGroupModifier { get; set; }
        public List<ItemGroupModifier> LstModifierGroup { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitModifierGroup();
        }

        private void InitModifierGroup()
        {

            ReadFromAloha();
        }

        private void ReadFromAloha()
        {
            try
            {
                LstModifierGroup = new List<ItemGroupModifier>();

                LasaFOHLib67.IberDepot depot = new LasaFOHLib67.IberDepotClass();
                LasaFOHLib67.IIberObject parent = depot.FindObjectFromId(740, CodeGroupModifier).First();

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

            LstModifierGroup = LstModifierGroup.OrderBy(e => e.Name).ToList();
            
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

            if(button == null)
                return;

            var item = button.Tag as ItemGroupModifier;
            
            if(item != null)
                FillModifierSection(item);
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
                                Price = item.GetStringVal("PRICE")
                            });
                        }
                        catch (Exception)
                        {
                            // expected
                        }
                    }
                }


                foreach (var modifier in listModifiers)
                {
                    var button = FactoryButton.CreateModifierButton(modifier);
                    WrModifier.Children.Add(button);
                }
            }
            catch (Exception)
            {
                // expected
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}

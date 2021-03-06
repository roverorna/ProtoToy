﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProtoEngine;
using System.IO.Ports;
using System.IO;

namespace KontrolerKomunikacyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Protocol protokol;
        private ArrayList daneListyPrototypow;
        ScrollViewer opcjeUrzadzeniaScroll = new ScrollViewer();
        StackPanel opcjeUrzadzeniaPanel = new StackPanel();
           
        int i = 0;
        Device zaznaczonyDevice;

        public MainWindow()
        {
            InitializeComponent();

             ZaladowanieNazwPortow();
             OtworzPrzycisk.TabIndex = 0;
             dodajUrzadzenieButton.TabIndex = 1;
             urzadzeniaListBox.TabIndex = 2;
             ustawOpcjeButton.TabIndex = 3;

            opcjeUrzadzeniaScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            opcjeUrzadzeniaScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Thickness thick = new Thickness(5, 590, 900, 0);
            opcjeUrzadzeniaScroll.Margin = thick;
            grid1.Children.Add(opcjeUrzadzeniaScroll);
            zaznaczonyDevice = null;

        }
        private void ZaladowanieNazwPrototypow()
        {
            daneListyPrototypow = new ArrayList();
            ArrayList list = new ArrayList();
            
            foreach (DevicePrototype device in protokol.DevicePrototypes)
                list.Add(device.Name);
            daneListyPrototypow = list;
            prototypyListBox.ItemsSource = null;
            prototypyListBox.ItemsSource = daneListyPrototypow;
            if(prototypyListBox.Items.Count>0)
            prototypyListBox.SelectedItem = prototypyListBox.Items[0];
        }
        private void ZaladowanieNazwPortow()
        {
            String[] ports = SerialPort.GetPortNames();
            foreach (String port in ports)
            {
                comboBox1.Items.Add(port);
            }
            comboBox1.Items.Add("COM1");
            comboBox1.Items.Add("COM2");
        }
        private void otworzPrzycisk(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "XML documents (.xml) | *.xml";
            dlg.FileName = "Modbus-serial";

            Nullable<bool> result = dlg.ShowDialog();
            String filename;
            if (result == true)
            {
                filename = dlg.FileName;
                protokol = new Protocol(filename);
                protocolLabel.Content += protokol.Name;
                ZaladowanieNazwPrototypow();
                ZaladowanieOpcjiProtokolu();
             
            }
        }
        private void ZaladowanieOpcjiProtokolu()
        {
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Thickness thick = new Thickness(10, 150,900, 500);
            scroll.Margin = thick;
            StackPanel panel = new StackPanel();
            foreach (Option option in protokol.Options)
            {
                CheckBox check = new CheckBox();
                check.Content = option.Name;
                panel.Children.Add(check);           
            }
          /*  for (int i = 0; i < 100; i++)
            {
                CheckBox check = new CheckBox();
                check.Content = "Teścik stu.";
                panel.Children.Add(check);         
            }*/
            scroll.Content = panel;
            grid1.Children.Add(scroll);
        }

     /*   private void WyslijPrzycisk_Click(object sender, RoutedEventArgs e)
        {
            Ramka ramka_master = new Ramka(2);


            byte adres, funkcja, adresWeWy, wartosc;
            int blad = 0;
            if (!byte.TryParse(AdresSlaveTextbox.Text, out adres)) //sprawdza, czy adres jest typu byte
            {
                BladMasterAdresLabel.Visibility = Visibility.Visible;
                blad = 1;
            }
            else
                BladMasterAdresLabel.Visibility = Visibility.Hidden;
            if (!byte.TryParse(FunkcjaTextbox.Text, out funkcja)) //sprawdza, czy kod funkcji jest typu byte
            {
                BladMasterFunkcjaLabel.Visibility = Visibility.Visible;
                blad = 1;
            }
            else
                BladMasterFunkcjaLabel.Visibility = Visibility.Hidden;
            if (funkcja == 1 || funkcja == 2)
            {
                if (!byte.TryParse(AdresWeWyTextbox.Text, out adresWeWy)) //TODO sprawdza czy adres we/wy jest typu 2*byte
                {
                    BladMasterAdresWeWyLabel.Visibility = Visibility.Visible;
                    blad = 1;
                }
                ramka_master.dane[1] = adresWeWy; //TODO zamienić dane[1]=adresWeWy na dane[0]=adresWeWy
            }
            if (funkcja == 5 || funkcja == 6) //jeśli kod funkcji oznacza zapis
            {
                if (!byte.TryParse(AdresWeWyTextbox.Text, out adresWeWy)) //TODO sprawdza czy adres we/wy jest typu 2*byte
                {
                    BladMasterAdresWeWyLabel.Visibility = Visibility.Visible;
                    blad = 1;
                }
                else
                    BladMasterAdresWeWyLabel.Visibility = Visibility.Hidden;
                if (!byte.TryParse(WartoscTextbox.Text, out wartosc)) //sprawdza, czy wartość we/wy jest typu byte
                {
                    BladMasterWartoscLabel.Visibility = Visibility.Visible;
                    blad = 1;
                }
                else
                    BladMasterWartoscLabel.Visibility = Visibility.Hidden;
                ramka_master.dane[1] = adresWeWy;
                ramka_master.dane[0] = wartosc;
            }
            ramka_master.adres = adres;
            ramka_master.funkcja = funkcja;


            ramka_master.SumaCRC(ramka_master.dane[0]);

            if (blad == 0)
            {
                String tmp = "Master [adres_slave kod_funkcji]: ";
                tmp += ramka_master.Wyswietl(true);
                Ramka ramka_slave = new Ramka(2); //TODO dlaczego nie =new Ramka(2)?
                for (int i = 0; i < listaSlave.Count; i++) //do każdego slave z listy
                {
                    ramka_slave = listaSlave[i].Receive(ramka_master); //wysyła ramkę od mastera
                    //slave zwraca ramkę odpowiedzi
                    if (ramka_slave.adres != 0)
                    {
                        tmp += "\nSlave [adres_slave kod_funkcji, ] " + listaSlave[i].adress.ToString() + ": ";
                        tmp += ramka_slave.Wyswietl(false);
                        ChatEtykieta.Content += "\n" + tmp;
                    }
                }
            }
        }
        */
    /*    private void RozmieszczeniePolRamek()
        {
            PolaRamki polaRamki = new PolaRamki();

            int marginesGorny = 47;
            int marginesLewy = 280;
            int marginesLewyTextbox = 360;
            double przes = AdresSlaveLabel.Height + 20;
            Thickness thickness;
            int i = 0;
            for (double tmp = marginesGorny; i < 5; i++, tmp += przes)
            {
                thickness = new Thickness(marginesLewy, tmp, 0, 0);
                Thickness thicknessTextbox = new Thickness(marginesLewyTextbox, tmp, 0, 0);
                if (i == 0)
                {
                    AdresSlaveLabel.Margin = thickness;
                    AdresSlaveLabel.Content = polaRamki.listaNazw[i];
                    AdresSlaveTextbox.Margin = thicknessTextbox;
                }
                else if (i == 1)
                {
                    FunkcjaLabel.Margin = thickness;
                    FunkcjaLabel.Content = polaRamki.listaNazw[i];
                    FunkcjaTextbox.Margin = thicknessTextbox;
                }
                else if (i == 2)
                {
                    DaneLabel.Margin = thickness;
                    DaneLabel.Content = polaRamki.listaNazw[i];

                }
                else if (i == 3)
                {
                    AdresWyWeLabel.Margin = thickness;
                    AdresWyWeLabel.Content = polaRamki.listaNazw[i];
                    AdresWeWyTextbox.Margin = thicknessTextbox;
                }
                else if (i == 4)
                {
                    WartoscLabel.Margin = thickness;
                    WartoscLabel.Content = polaRamki.listaNazw[i];
                    WartoscTextbox.Margin = thicknessTextbox;
                }
            }
        }
        */
       /* private void dodajUrzadzenie_Click(object sender, RoutedEventArgs e)
        {
            byte adres;
            int iloscWejsc, iloscWyjsc;
            if (prototypyListBox.SelectedItem != null)
            {
                if (!byte.TryParse(DodajAdresTextbox.Text, out adres))
                    BladAdresSlaveLabel.Visibility = Visibility.Visible;
                else
                    BladAdresSlaveLabel.Visibility = Visibility.Hidden;
                if (!int.TryParse(IloscWejscTextbox.Text, out iloscWejsc))
                    BladIloscWejscLabel.Visibility = Visibility.Visible;
                else
                    BladIloscWejscLabel.Visibility = Visibility.Hidden;
                if (!int.TryParse(IloscWyjscTextbox.Text, out iloscWyjsc))
                    BladIloscWyjscLabel.Visibility = Visibility.Visible;
                else
                    BladIloscWyjscLabel.Visibility = Visibility.Hidden;
                if (BladAdresSlaveLabel.Visibility == Visibility.Hidden
                    && BladIloscWejscLabel.Visibility == Visibility.Hidden
                    && BladIloscWyjscLabel.Visibility == Visibility.Hidden)
                {
                    Slave slave = new Slave(prototypyListBox.SelectedValue.ToString(), adres, iloscWejsc, iloscWyjsc);
                    bool jest = false;
                    for (int i = 0; i < listaSlave.Count(); i++)
                        if (listaSlave[i].adress == slave.adress)
                            jest = true;
                    if (!jest)
                        listaSlave.Add(slave);
                    else
                        MessageBox.Show("Slave o wybranym adresie już istnieje");

                    UrzadzeniaWSystemieLabel.Content = "";
                    for (int i = 0; i < listaSlave.Count; i++)

                        UrzadzeniaWSystemieLabel.Content += listaSlave[i].Wyswietl();
                }
            }
            else MessageBox.Show("Zaznacz prototyp");

        }

     */
        private void dodajUrzadzenieButton_Click(object sender, RoutedEventArgs e)
        {
             if (prototypyListBox.SelectedItem != null)
            {
                 DevicePrototype proto=ZwrocDevicePrototype(prototypyListBox.SelectedItem.ToString());
                 if (proto != null)
                 {
                     protokol.registerDevice(proto.create());
                     int indeks=protokol.RegisteredDevices.Count-1;
                     protokol.RegisteredDevices[indeks].Name=prototypyListBox.SelectedValue.ToString()+" "+i;
                     urzadzeniaListBox.Items.Add(prototypyListBox.SelectedValue.ToString()+" "+i++);
                     urzadzeniaListBox.SelectedItem = urzadzeniaListBox.Items[0];
                 }
                 else
                     MessageBox.Show("Nie znaleziono na liscie prototypow.");
                 }
            else MessageBox.Show("Zaznacz prototyp");
        }

        private DevicePrototype ZwrocDevicePrototype(String nazwa) {

            foreach (DevicePrototype proto in protokol.DevicePrototypes)
                if (proto.Name == nazwa)
                    return proto;
            return null;
    }
        private bool EkstraPorownanie(String krotszy, String dluzszy)
        {
            int i = 0;
            foreach (char ch in krotszy)
            {
                if (ch == dluzszy[i++]) ;
                else return false;
            }
            return true;
        }
        private Device ZwrocDevice(String nazwa)
        {

            foreach (Device dev in protokol.RegisteredDevices)
            {
                 // if (EkstraPorownanie(dev.Name,nazwa))
               
                  if(dev.Name==nazwa)
                return dev;
            }
            return null;
        }

        private void usunUrzadzenieButton_Click_1(object sender, RoutedEventArgs e)
        {
             if (urzadzeniaListBox.SelectedItem != null)
            {
                 protokol.unregisterDevice(ZwrocDevice(urzadzeniaListBox.SelectedItem.ToString()));
                 urzadzeniaListBox.Items.RemoveAt(urzadzeniaListBox.Items.IndexOf(urzadzeniaListBox.SelectedItem));
                 opcjeUrzadzeniaScroll.Content = null;
                 opcjeUrzadzeniaPanel = new StackPanel(); 
                 prototypeLabel.Content = null;
                 zaznaczonyDevice = null;
            }
             else MessageBox.Show("Zaznacz urządzenie");
        }
        private void ZaladowanieOpcjiUrzadzenia(Device device,String s)
        {
            opcjeUrzadzeniaPanel = null;
            opcjeUrzadzeniaPanel = new StackPanel();
            prototypeLabel.Content = s;
            foreach (Option option in device.Options)
            {
                Label label = new Label();
                label.Content = option.Name;
                opcjeUrzadzeniaPanel.Children.Add(label);
                TextBox text = new TextBox();
                text.Text = option.getValueAsString();
                opcjeUrzadzeniaPanel.Children.Add(text);
            }
            opcjeUrzadzeniaScroll.Content = opcjeUrzadzeniaPanel;
        }
        private void urzadzeniaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox) != null)
                if ((sender as ListBox).SelectedValue != null)
                {
                    zaznaczonyDevice = ZwrocDevice((sender as ListBox).SelectedValue.ToString());
                    if (zaznaczonyDevice != null)
                    {
                        ZaladowanieOpcjiUrzadzenia(zaznaczonyDevice, (sender as ListBox).SelectedValue.ToString());
                     
                    }
                }    
        }

        private void ustawOpcjeButton_Click(object sender, RoutedEventArgs e)
        {
            if (zaznaczonyDevice != null)
            {
                int i = 1;
                int blad = 0;
               foreach (Option option in zaznaczonyDevice.Options)
                {
                    
                   TextBox text=(opcjeUrzadzeniaPanel.Children[i] as TextBox);
                   if (text.Text != "" && text.Text != " ")
                   {
                       i += 2;
                       option.setValueFromString(text.Text);
                   }
                   else blad = 1;
                        
                }
                 if(blad==1) MessageBox.Show("Uzupełnij wszystkie pola opcji urządzenia.");
            }
        }
        private byte[] SpakujWszystkoDoTablicyBajtow()
        {
            byte[] buffer=new byte[257]; //maksymalna wartość przesyłanej ramki w MODBUS-ie
            int i = 0;
            byte adres, funkcja, adresWy, wartoscWy;
            int blad = 0;
            if (!byte.TryParse(AdresTextbox.Text, out adres)) //sprawdza, czy adres jest typu byte
            {
                MessageBox.Show("Adres musi być typu byte.");
                blad = 1;
                buffer[i++] = adres;
            }
            if (!byte.TryParse(FunkcjaTextbox.Text, out funkcja)) //sprawdza, czy kod funkcji jest typu byte
            {
                MessageBox.Show("Funkcja musi być typu byte.");
                blad = 1;
                buffer[i++] = funkcja;
            }
            if (!byte.TryParse(AdresWyTextbox.Text, out adresWy)) //TODO sprawdza czy adres we/wy jest typu 2*byte
            {
                buffer[i++] = adresWy;
            }
            if (!byte.TryParse(WartoscTextbox.Text, out wartoscWy)) //sprawdza, czy wartość we/wy jest typu byte
            {
                buffer[i++] = wartoscWy;
            }
           // buffer[i++] = Crc_MlodszyBajt();
           // buffer[i++] = Crc_StarszyBajt();
            if (blad == 1) return null;
            return buffer;
        }
        private String SpakujWszystkoDoTablicyStringow()
        {
            String buffer;
            byte adres,funkcja,adresWy,wartoscWy;
            byte.TryParse(AdresTextbox.Text, out adres); //sprawdza, czy adres jest typu byte         
            buffer = adres.ToString();
            byte.TryParse(FunkcjaTextbox.Text, out funkcja);
            buffer+=","+funkcja.ToString();
            byte.TryParse(AdresWyTextbox.Text, out adresWy);
            buffer += "," + adresWy.ToString();
            byte.TryParse(WartoscTextbox.Text, out wartoscWy);
            buffer += "," + wartoscWy.ToString();
            return buffer;

        }
        private void WyswietlPodglad(byte[] ramkaMaster,byte[] ramkaSlave,String nazwaUrzadzenia)
        {
            String tmp = "Master: ";
            ChatEtykieta.Content += "\n" + tmp+SpakujWszystkoDoTablicyStringow();
            String tmp2 = nazwaUrzadzenia+": ";
            ChatEtykieta.Content += "\n" + tmp2+SpakujWszystkoDoTablicyStringow();
        }
        private Device Wyslij(String adres)
        {
      
            byte[] ramka;
            foreach (Device device in protokol.RegisteredDevices)
            {
                if (adres == device.Options[0].getValueAsString())
                {
                    return device; 
                }
            }
            return null;
        }
        private void WyslijPrzycisk_Click(object sender, RoutedEventArgs e)
        {
            byte[] buffer=SpakujWszystkoDoTablicyBajtow();
            if (buffer != null)
            {
               // Stream wejscie = new MemoryStream(buffer);
              //  wejscie.Seek(0, SeekOrigin.Begin);
             //   Stream wyjscie = new MemoryStream();
            //   protokol.run(wejscie, wyjscie);
                byte[] ramka=null;
                Device device=Wyslij(AdresTextbox.Text);
                if(device!=null)
                    WyswietlPodglad(buffer,ramka,device.Name);
                else
                    MessageBox.Show("Nie ma urządzenia o takim adresie.");
            }
        }

  

        

      


       

    }
}
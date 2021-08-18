using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeleSharp.TL;
using TLSharp.Core;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        string nomer = "79152142851";
        int apiId = 7777870;
        String apiHash = "d35366d27895f506b459aca6ecf16abc";
        TelegramClient client;
        string hash;
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                TLSharp.Core.FileSessionStore store = new TLSharp.Core.FileSessionStore();

                client = new TelegramClient(apiId, apiHash, store, "session");
                //if your app is not autenticated by telegram this code will send  request to add a new device then telegram will sent you a Autenticatin code
                await client.ConnectAsync();
                MessageBox.Show("Ok");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                hash = await client.SendCodeRequestAsync(nomer);

            }

        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                //if your app is not autenticated by telegram and you have sent request to telegram, this code will add new device using autenticatin code that telegram sent to you
                var code = txtAutCode.Text;  // this is a TextBox that you must insert the code that Telegram sent to you
                var user = await client.MakeAuthAsync(nomer, hash, code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            getContact();



        }
        public async void SendMes()
        {
            await client.ConnectAsync();

            // Here is the code that will add a new contact and send a test message
            TeleSharp.TL.Contacts.TLRequestImportContacts requestImportContacts = new TeleSharp.TL.Contacts.TLRequestImportContacts();
            requestImportContacts.Contacts = new TLVector<TLInputPhoneContact>();
            requestImportContacts.Contacts.Add(new TLInputPhoneContact()
            {
                Phone = "new Number in global format example : 98916*******",
                FirstName = "new Number's FirstName",
                LastName = "new Number's LastName"
            });
            var o = await client.SendRequestAsync<TeleSharp.TL.Contacts.TLImportedContacts>((TLMethod)requestImportContacts);
            var NewUserId = (o.Users.First() as TLUser).Id;
            var d = await client.SendMessageAsync(new TLInputPeerUser() { UserId = NewUserId }, "test message text");



            //find recipient in contacts and send a message to it
            var result = await client.GetContactsAsync();
            var user = result.Users
                .Where(x => x.GetType() == typeof(TLUser))
                .Cast<TLUser>()
                .FirstOrDefault(x => x.Phone == "recipient Number in global format example : 98916*******");

            MessageBox.Show((user.LastName).ToString());

            //send message
            await client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, "hi test message");
        }
        public async void getContact()
        {
           
            var result = await client.GetContactsAsync();
          
       
            string ss1=String.Empty;
          
            
            foreach (var d in result.Contacts)
            {
                var user = result.Users
              .Where(x => x.GetType() == typeof(TLUser))
              .Cast<TLUser>()
              .FirstOrDefault(x => x.Id == d.UserId);
                if(user!=null )
                {
                    try
                    {
                        if(user.FirstName != null && user.LastName != null)
                        {
                            ss1 += (user.FirstName).ToString() + "\t"+ (user.LastName).ToString()+"\n";
                        }
                        else
                        {
                            if ( user.FirstName != null)
                            {
                                try
                                {


                                    ss1 += (user.FirstName).ToString() + "\n";
                                }
                                catch (Exception)
                                {

                                }
                            }
                            if (user.LastName != null)
                            {
                                try
                                {


                                    ss1 += (user.LastName).ToString() + "\n";
                                }
                                catch (Exception)
                                {

                                }
                            }

                        }
                        
                    }
                    catch(Exception)
                    {

                    }
                }
                



            }
            tt1.Text = ss1;

        }
    }
}

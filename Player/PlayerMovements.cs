using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovements : MonoBehaviour
{
    //=====FLOAT=====
    float PlayerForwardSpeed = 6f; // Player'ýn ileriye doðru gidiþ hýzý deðiþkeni
    float PlayerTurnSpeed = 20f; // Player saða veya sola açýsal dönüþ hýzý
    float PlayerspeedModifier = 0.01f; // Touch haraketi için hýz kat sayýsý
    float leftBorder1, rightBorder1, leftBorder2, rightBorder2; // Karakter haraket halindeyken saða ve sola taþmalarý önlemek için temas ettiði yüzeyin koordinatlarýný alarak +2 veya -2 deðerler arasý haraketini saðlamak.
    float MoneyCounterWaitTime = 0.4f; // UI üzerinde para veya bira toplayýnca çýkan textin ekranda kalma süresi kontrolü
    float timer = 0.0f; // UI üzerinde para veya bira için toplanýldýktan sonra timer sayesinde kaç saniye kalabileceðini kontrol etmek.

    //=====BOOL=====
    bool TurnRight,TurnLeft = false; // Saða veya Sola dönüþ için trigger objesi sayesinde kontrol saðlama.
    bool RayCastControl = false; // Bastýðý zemin ile Player arasýndaki mesafeyi kontrol ederek iþlem yapmak için kullanýlan bool deðiþkeni
    bool GameStart = false; // Oyuna baþlamak için, Start butonuna basýldýðýnda oyundaki fonksiyonlarý baþlatmak için kullanýlan bool deðiþkeni
    bool MoneyTimer = false; // Dollar veya Biralara çarptýðýnda Time.deltatime'ý çalýþtýrma (+ veya - para toplama yazýlarýný ekranda tutulmasý)
    bool TouchMovement = true; // Oyun içerisinde saða ve sola dönüþ olduðunda touch iþlemini kapatma ve açmak için kullanýlýyor.
    bool PlayerMovement = true; // Player'ýn ileriye doðru haraketi için kullanýlýr.

    //=====INT=====
    int collectibleObjectCounter = 0; // Toplanan objelerin sayýsýný tutar.
    int CounterPositiveForUI = 0; // Ekranda Positif nesne topladýðýmýzda gösterilecek olan Textlerin deðerini tutan deðiþken.
    int CounterNegativeForUI = 0; // Ekranda Negatif nesne topladýðýmýzda gösterilecek olan Textlerin deðerini tutan deðiþken.
    int DollarTotal = 0; // Baþlangýçtaki 20 caný ve daha sonra toplanacak olan objelerin deðerlerini birlikte tutarak ekrandaki level yazýsýnýn altýndaki text verisine basýlan deðer.
    int LookTo = 0; // 0(ileri) 1(sað) 2(geri) 3(sol) // Karakterin hangi posisyonda olduðunu algýlayarak buna göre iþlem yaptýrýlýr.
    int finishDoorCounter = 0; // 1(poor) 2(avarage) 3(Rich) // Oyun bitiþ kapýlarýndaki triggerlar sayesinde oyunu hangi evrede bitirdiðimizi kontrol eden sayaç.
    int currentHealth = 20; // Slider için baþlangýç caný.

    //=====STRING=====
    string DollarDefaultNumber = "20"; // Karakter canýnýn baþlangýçtaki canýný 20 olarak belirleyerek. Bundan sonraki toplama iþlemlerini bu deðiþken ile gerçekleþir.
    

    // ===== ANIMATOR =====
    [SerializeField] Animator PlayerAnim; // Karakterin animasyon kontrolcüsü
    [SerializeField] Animator Door1Left, Door1Right, Door2Left, Door2Right, Door3Left, Door3Right; // Bitiþ çizgisindeki kapýlarýn animasyon kontrolcüleri.

    // ===== GAMEOBJECT =====
    [SerializeField] GameObject StartButton, RestartBtn; // Oyuna baþlamak için kullanýlan button // Oyuna yeniden baþlamak için kullanýlan button.
    [SerializeField] GameObject DollarCounterGameobjectPositive, DollarCounterGameobjectNegative; // Oyun ekranýnda negatif veya positif herhangi bir þey topladýðýmýzda ekrana yeni textler çýkartarak haraket ettirmek için kullanýlan textler
    [SerializeField] GameObject PlayerCanvas; // Ekrana çýkartýlacak negatif ve positif textleri Player içerisindeki Canvas'ýn Child objeleri yapmak için kullanýlan gameobject.
    [SerializeField] GameObject MoneyPositionPositive,MoneyPositionNegative; // Ekranda nesne topladýðýmýzda belirecek olan Negatif ve Positif textlerin spawn olacaðý pozisyonlar.
    [SerializeField] GameObject Poor, Avarage, Rich; // Karakter durumuna göre karakterin fakir,ortalama veya zengin hallerinin görüntüsünü açýp kapatmak için kullanýlan objeler.
    [SerializeField] GameObject Fill; // Slider'ýn içerisindeki azalan veya artan çubuða ulaþarak rengini deðiþtirmek için kullanýlýr.
    [SerializeField] GameObject MoneyCounterUI; // Level yazýsýnýn altýndaki o anki canýmýzý gösteren counter gameobjectine ulaþarak içerisindeki veriyi almak için kullanýlýr.
    [SerializeField] GameObject FinishMoneyTxt; // Oyun bittiðinde geçilen kapý sayýsýna göre X(çarpý) bonus alarak kazanýlan dolar miktarýný ekrana yazdýrmak için ulaþýlan gameobject.

    // ===== OTHER =====
    Touch TheTouch; // Ekran haraketlerini algýlamak için Touch.
    RaycastHit hit; // Karakterin hangi zemin ile temas ettiðini kontrol etmek için oluþturulan RaycastHit. Bunun sayesinde karakterin bulunduðu zemindeki sýnýrlarýný belirleyebiliyoruz.
    public SliderControl sliderControl; // Ekrandaki can slideri 
    [SerializeField] private TextMeshProUGUI DollarCountertxtPositive, DollarCountertxtNegative; // UI'da belirecek olan Text'lere proje alanýndan ulaþma.
    [SerializeField] TextMeshProUGUI PlayerStatusTxt; // Karakterin o anki durumunu sakladýðýmýz txt verisi.
    void Start()
    {

    }
    void Update()
    {
        if (GameStart)
        {
            PlayerWalk(); // Player'ýn ileriye haraketi
            PlayerTurns(); // Player'ýn saða veya sola dönüþ yaptýrma iþlemleri
            PlayerTouchControl(); // Player'ý haraket ettirme iþlemleri
            PlayerStatusControl(); // Player'ýn o anki durumunu kontrol etme iþlemleri
            PlayerAnimationControl(); // Player'ýn durumlara göre animasyonlarýný çalýþtýrma iþlemleri
            PlayerDeadControl(); // Player öldüðünde çalýþacak iþlemler.
        }
        if (MoneyTimer)
        {
            timer += Time.deltaTime;
        }
    }
    void PlayerWalk()
    {
        if (PlayerMovement)
        {
            transform.Translate(Vector3.forward * PlayerForwardSpeed * Time.deltaTime); // Karakterin ileriye doðru haraketini saðlar.
        }
    }
    void PlayerDeadControl()
    {
        if (int.Parse(MoneyCounterUI.GetComponent<TextMeshProUGUI>().text) <= 0) // Player'ýn caný 0'a indiðinde veya daha düþük olduðunda sahneyi yeniden yükler.
        {
            SceneManager.LoadScene(0);
        }
    }
    void PlayerTurns()
    {
        // LookTo 0(ileri) 1(sað) 2(geri) 3(sol)
        if (TurnRight) // Karakter saða dönüþ alanýna gelince Trigger sayesinde çalýþýr.
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), PlayerTurnSpeed * Time.deltaTime); // Player'ýn Y rotasyonunu 90 dereceye belirlenen zaman çarpaný boyunca çevirir.
            if (transform.localRotation.eulerAngles.y >= 89f) // Player'ý zaman çarpaný ile çevirdiðimizden dönüþ eksik veya fazla olmasý durumunda Player rotasyonu 90'a eþitlenir.
            {
                TurnRight = false;
                transform.rotation = Quaternion.Euler(transform.rotation.x, 90f, transform.rotation.z);
                LookTo +=1;
                if (LookTo < 0)
                {
                    LookTo += 4;                         // LookTo deðiþkeni ile oluþturulan fonksiyon sayesinde karakterin hangi posisyonda olduðunu tespit ederek hangi Koordinattaki Touch verilerinin çalýþacaðý belirlenir.
                }
                else if (LookTo >= 4)
                {
                    LookTo %= 4;
                }
            }
        }
        if (TurnLeft) // Karakter sola dönüþ alanýna gelince Trigger sayesinde çalýþýr.
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), PlayerTurnSpeed * Time.deltaTime); // Player'ýn Y rotasyonunu 0 dereceye belirlenen zaman çarpaný boyunca çevirir.
            if (transform.localRotation.eulerAngles.y <= 1f)
            {
                TurnLeft = false;
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z); // Player'ý zaman çarpaný ile çevirdiðimizden dönüþ eksik veya fazla olmasý durumunda Player rotasyonu 90'a eþitlenir.
                LookTo -= 1;
                if (LookTo < 0)
                {
                    LookTo += 4;                         // LookTo deðiþkeni ile  oluþturulan fonksiyon sayesinde karakterin hangi posisyonda olduðunu tespit ederek hangi Koordinattaki Touch verilerinin çalýþacaðý belirlenir.
                }
                else if (LookTo >= 4)
                {
                    LookTo %= 4;
                }
            }
        }
    }
    void PlayerTouchControl()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10.0f)) // Karakter'den  yere doðru 10 büyüklüðünde çizgi gönderilir.
        {
            RayCastControl = true; // Bu çizgi gönderilerek Player hangi zemin ile temas ediyorsa onun transform verilerine ulaþarak Player'ýn haraket edeceði konumlar oluþturulur.
        }     
        if (Input.touchCount > 0)
        {
            TheTouch = Input.GetTouch(0);                       // Ekranda Dokunma haraketleri olduðunda çalýþan iþlemler.
            if (TheTouch.phase == TouchPhase.Moved)
            {
                // LookTo 0(ileri) 1(sað) 2(geri) 3(sol)
                if (LookTo == 0 && TouchMovement == true)       // LookTo sayesinde hangi posisyonda olduðumuzu belirleyerek buna uygun haraket iþlemleri uygulanýr.
                {
                    leftBorder1 = hit.collider.transform.position.x + 2f;      //Raycast sayesinde Player'ýn temas ettiði zeminin Transform bilgilerine ulaþarak o zemin üzerinde karakter maksimum/minimum +2 veya -2 büyüklükte haraket saðlar.
                    rightBorder1 = hit.collider.transform.position.x - 2f;
                    if (RayCastControl && transform.position.x <= leftBorder1 && transform.position.x >= rightBorder1)
                    {
                        transform.position = new Vector3(transform.position.x + TheTouch.deltaPosition.x * PlayerspeedModifier, transform.position.y, transform.position.z); // Karakterin saða veya sola haraket iþlemleri
                    }
                    if (transform.position.x > leftBorder1)
                    {
                        transform.position = new Vector3(leftBorder1, transform.position.y, transform.position.z); // Herhangi bir þekilde maksimum veya minimum haraket alanlarý aþýlýrsa minimum veya maksimum haraket alanlarýna geri dönülür.
                    }
                    if (transform.position.x < rightBorder1)
                    {
                        transform.position = new Vector3(rightBorder1, transform.position.y, transform.position.z);
                    }
                }
                if (LookTo == 1 && TouchMovement == true)
                {
                    leftBorder2 = hit.collider.transform.position.z + 2f;      //Raycast sayesinde Player'ýn temas ettiði zeminin Transform bilgilerine ulaþarak o zemin üzerinde karakter maksimum/minimum +2 veya -2 büyüklükte haraket saðlar.
                    rightBorder2 = hit.collider.transform.position.z - 2f;
                    if (RayCastControl && transform.position.z <= leftBorder2 && transform.position.z >= rightBorder2)
                    { 
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - TheTouch.deltaPosition.x * PlayerspeedModifier); // Karakterin saða veya sola haraket iþlemleri
                    }
                    if (transform.position.z > leftBorder2)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, leftBorder2); // Herhangi bir þekilde maksimum veya minimum haraket alanlarý aþýlýrsa minimum veya maksimum haraket alanlarýna geri dönülür.
                    }
                    if (transform.position.z < rightBorder2)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, rightBorder2);
                    }
                }   
            }
        }
    }
    void PlayerAnimationControl()
    {
        PlayerAnim.SetBool("ÝdleToPoor", true); // Karakterin baþlangýçtaki idle animasyonu aktif etmek için kullanýlýr.
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "rotateRightTrigger") // Saða doðru dönüþü kontrol etmek için kullanýlan Trigger Objesi
        {
            TouchMovement = false;
            TurnRight = true;
            StartCoroutine(WaitingFuction()); // Belirli süre beklenerek yapýlan iþlemler için kullanýlan fonksiyon.
        }
        if (other.gameObject.tag == "rotateLeftTrigger") // Sola doðru dönüþü kontrol etmek için kullanýlan Trigger Objesi
        {
            TouchMovement = false;
            TurnLeft = true;
            StartCoroutine(WaitingFuction());   // Belirli süre beklenerek yapýlan iþlemler için kullanýlan fonksiyon.
        }
        if(other.gameObject.tag == "dollar") // Dollar nesnesini topladýðýný kontrol etmek için kullanýlan Trigger Objesi
        {
            TakeHeal(2);
            collectibleObjectCounter+=2;
            CounterPositiveForUI += 2;                  // Çarpýlan nesne dolar ise deðer 2 artar ve kendini imha eder. Ayrýca WallTriggerPositive fonksiyonu çalýþýr.
            Destroy(other.gameObject);
            WallTriggerPositive();
        }
        else if(other.gameObject.tag == "beer") // Bira nesnesini topladýðýný kontrol etmek için kullanýlan Trigger Objesi
        {
            TakeDamage(2);
            collectibleObjectCounter-= 2;               // Çarpýlan nesne beer ise deðer 2 azalýr ve kendini imha eder. Ayrýca WallTriggerPositive fonksiyonu çalýþýr.
            CounterNegativeForUI -= 2; 
            Destroy(other.gameObject);
            WallTriggerNegative();
        }
        else if(other.gameObject.tag == "GreenTrigger")  // Yeþil alan içerisinden geçerek +20 puan aldýðýný kontrol etmek için kullanýlan Trigger Objesi
        {
            TakeHeal(20);
            collectibleObjectCounter+= 20;
            CounterPositiveForUI += 20;
            Destroy(other.gameObject);                  // Çarpýlan nesne Green alan ise deðer 20 artar ve kendini imha eder. Ayrýca WallTriggerPositive fonksiyonu çalýþýr.
            WallTriggerPositive();
            CounterPositiveForUI = 0;
        }
        else if(other.gameObject.tag == "RedTrigger") // Kýrmýzý alan içerisinden geçerek -20 puan aldýðýný kontrol etmek için kullanýlan Trigger Objesi
        {
            TakeDamage(20);
            collectibleObjectCounter-= 20;
            CounterNegativeForUI -= 20;
            Destroy(other.gameObject);                  // Çarpýlan nesne Red alan ise deðer 20 azalýr ve kendini imha eder. Ayrýca WallTriggerPositive fonksiyonu çalýþýr.
            WallTriggerNegative();
            CounterNegativeForUI = 0;
        }
        if (other.gameObject.tag != "beer" || other.gameObject.tag != "dollar" || other.gameObject.tag != "RedTrigger" || other.gameObject.tag != "GreenTrigger")
        {
            if (timer > MoneyCounterWaitTime)
            {
                CounterPositiveForUI = 0;
                CounterNegativeForUI = 0;                   // Timer ile birlikte toplanan dolarlarýn ekranda stackleneceði süresini ayarlama iþlemleri ve belirli sürenin üzerinde stacklenmenin tekrar sýfýrlanmasý.
                MoneyTimer = false;
                timer = 0.0f;
            }
        }
        if (other.gameObject.tag == "FinishDoor") // Bitiþ çizgisinde bulunan 3 farklý Trigger sayesinde Player'ýn o anki canýna veya dolarýna göre oyunu hangi seviyede bitireceðini belirleyerek iþlemler yaptýrýlýr.
        {
            finishDoorCounter++; // 1(poor) 2(avarage) 3(Rich) // 3 farklý bitiþ çizgisindeki Triggerlara çarparak counter sayýsý kontrol ettirilir. Buna göre hangi seviyede olduðu tespit edilir.
            TouchMovement = false;
            Destroy(other.gameObject);
            if (sliderControl.slider.value <= 40 && finishDoorCounter ==1)
            {
                // Üzülme animasyonu oynatýlacak.
                // Para 1x olarak aktarýlacak.
                // Kapý açýlmayacak.
                PlayerMovement = false; // Player haraketi engellenir.
                PlayerAnim.SetBool("Loop", true); // Üzülme animasyonu deðiþkeni True yapýlarak üzülme animasyonu oynatýlýr.
                FinishMoneyTxt.GetComponent<TextMeshProUGUI>().text = (DollarTotal).ToString(); // Oyun sonunda topladýðýmýz para 1x ile çarpýlýr kasaya aktarýlýr.
                RestartBtn.SetActive(true); // Tek levelden oluþtuðu için level baþlangýcýna dönmek için Restart butonu kullanýlýr. Bu buton aktif hale getirilir.
            } 
            else if(sliderControl.slider.value > 40 && finishDoorCounter == 1)
            {
                // 1.Kapý açýlacak.
                Door1Left.SetInteger("New Int", 1);                 // 1. kapý içerisindeki animasyon deðerleri arttýrýlarak animasyonun oynatýlmasý saðlanýr.
                Door1Right.SetInteger("New Int", 1);
            }
            if(sliderControl.slider.value <= 70 && finishDoorCounter == 2)
            {
                PlayerMovement = false; // Player haraketi engellenir.
                // 2.Kapý açýlmadý ve oyun bitti.
                PlayerAnim.SetBool("Loop", true);   // Sevinme animasyonu deðiþkeni True yapýlarak üzülme animasyonu oynatýlýr.
                FinishMoneyTxt.GetComponent<TextMeshProUGUI>().text = (DollarTotal*2).ToString();   // Oyun sonunda topladýðýmýz para 2x ile çarpýlýr kasaya aktarýlýr.
                RestartBtn.SetActive(true);
            }
            else if (sliderControl.slider.value > 70 && finishDoorCounter == 2)
            {
                // 2. Kapý açýldý.
                Door2Left.SetInteger("New Int", 1);                 // 2. kapý içerisindeki animasyon deðerleri arttýrýlarak animasyonun oynatýlmasý saðlanýr.
                Door2Right.SetInteger("New Int", 1);
            }
            if (sliderControl.slider.value > 70 && finishDoorCounter == 3)
            {
                // 3. Kapý açýldý ve oyun bitti.
                Door3Left.SetInteger("New Int", 1);
                Door3Right.SetInteger("New Int", 1);            // 3. kapý içerisindeki animasyon deðerleri arttýrýlarak animasyonun oynatýlmasý saðlanýr.
                StartCoroutine(ThirdFinishFuntion());            // 3. kapý açýldýktan belirli süre sonra çalýþacak fonksiyon.
            }
        }
    }
    void TakeDamage(int damage)     // Hasar aldýðýmýzda slidera veri göndermezi saðlayan fonksiyon.
    {
        currentHealth -= damage;
        sliderControl.SetHealth(currentHealth);
    }
    void TakeHeal(int damage)       // Ýyileþtirme aldýðýmýzda slidera veri göndermezi saðlayan fonksiyon.
    {
        currentHealth += damage;
        sliderControl.SetHealth(currentHealth);
    }
    void PlayerStatusControl()
    {
        if (sliderControl.slider.value <= 40f)          // Karakterin slider value deðerine göre slider renginin ayarlanmasý, karakter görünümleri aktifleþtirilmesi, karakter durum yazýlarý iþlemleri 
        {
            PlayerStatusTxt.text = "Poor";
            Poor.SetActive(true);
            Avarage.SetActive(false);
            Rich.SetActive(false);
            PlayerStatusTxt.color = Color.red;
            Fill.GetComponent<Image>().color = Color.red;
        }
        else if (sliderControl.slider.value <= 70f)     // Karakterin slider value deðerine göre slider renginin ayarlanmasý, karakter görünümleri aktifleþtirilmesi, karakter durum yazýlarý iþlemleri 
        {
            PlayerStatusTxt.text = "Average";
            Poor.SetActive(false);
            Avarage.SetActive(true);
            Rich.SetActive(false);
            PlayerStatusTxt.color = new Color32(239, 148, 15, 255);
            Fill.GetComponent<Image>().color = new Color32(239, 148, 15, 255);
        }
        else
        {                                               // Karakterin slider value deðerine göre slider renginin ayarlanmasý, karakter görünümleri aktifleþtirilmesi, karakter durum yazýlarý iþlemleri 
            Poor.SetActive(false);
            Avarage.SetActive(false);
            Rich.SetActive(true);
            PlayerStatusTxt.text = "Rich";
            PlayerStatusTxt.color = Color.green;
            Fill.GetComponent<Image>().color = Color.green; 
        }
    }
    void WallTriggerPositive() // Bu fonksiyon Positif bir nesneye çarpýldýðýnda aktif olarak çalýþýr. Genel amacý toplanan paralarýn miktarýný ekranda göstermektir.
    {
        DollarTotal = (int.Parse(DollarDefaultNumber) + (collectibleObjectCounter)); // Baþlangýç caný olan 20 ile birlikte toplanan nesneyi deðiþken içerisinde tutarak ekrana yazdýrma iþlemini yaptýrýrýz.
        DollarCountertxtPositive.text = DollarTotal.ToString(); // Nesneleri topladýðýmýzda ekranda bir kaç saniye beliren textlerin içerine deðer yazarýz.
        MoneyCounterUI.GetComponent<TextMeshProUGUI>().text = DollarTotal.ToString(); // Ekrandaki level yazýsýn altýndaki para deðerini yazar.
        MoneyTimer = true;  // Bu deðer True konuma geçtiðinde bir Timer harakete geçer ve belirli süre boyunca oyun içerisindeki paralar stacklanýr. Belirli zaman sonra toplama olmaz ise sýfýrlanýr.
        PlayerAnim.SetInteger("Dcounter", DollarTotal); // Player içerisindeki sevinme veya üzülme animasyonlarý DollarTotal'den alýnan verilere göre çalýþýr.

        var myNewSmoke = Instantiate(DollarCounterGameobjectPositive, MoneyPositionPositive.transform.position, Quaternion.identity); // Yeni bir Pozitif Txt gameobjecti oluþturulur. 
        myNewSmoke.transform.SetParent(PlayerCanvas.gameObject.transform);                                                            // Player içerisindeki Canvas altýnda olmasý saðlanýr.
        myNewSmoke.transform.localScale = new Vector3(1f, 1f, 1f);                                                                    // Boyutu belirlenir.
        myNewSmoke.GetComponent<RectTransform>().DOLocalMoveY(10f, 0.5f).OnComplete(() => Destroy(myNewSmoke));                       // Pozitif olan txt dosyasý belirlenen konuma yumuþak þekilde haraket eder ve kaybolur.

        myNewSmoke.GetComponent<TextMeshProUGUI>().text = "+ " + CounterPositiveForUI.ToString() + " $";                              // Sayýnýn baþýna ve sonuna uygun iþaretler eklenir.
    }
    void WallTriggerNegative()  // Bu fonksiyon Negatif bir nesneye çarpýldýðýnda aktif olarak çalýþýr. Genel amacý toplanan paralarýn miktarýný ekranda göstermektir.
    {
        DollarTotal = (int.Parse(DollarDefaultNumber) + (collectibleObjectCounter));
        DollarCountertxtNegative.text = DollarTotal.ToString();
        MoneyCounterUI.GetComponent<TextMeshProUGUI>().text = DollarTotal.ToString();
        MoneyTimer = true;
        PlayerAnim.SetInteger("Dcounter", DollarTotal);

        var myNewSmoke = Instantiate(DollarCounterGameobjectNegative, MoneyPositionNegative.transform.position, Quaternion.identity);   // Yeni bir Negatif Txt gameobjecti oluþturulur. 
        myNewSmoke.transform.SetParent(PlayerCanvas.gameObject.transform);                                                              // Player içerisindeki Canvas altýnda olmasý saðlanýr.
        myNewSmoke.transform.localScale = new Vector3(1f, 1f, 1f);                                                                      // Boyutu belirlenir.
        myNewSmoke.GetComponent<RectTransform>().DOLocalMoveY(10f, 0.5f).OnComplete(() => Destroy(myNewSmoke));                         // Negatif olan txt dosyasý belirlenen konuma yumuþak þekilde haraket eder ve kaybolur.

        myNewSmoke.GetComponent<TextMeshProUGUI>().text = "- " + CounterNegativeForUI.ToString() + " $";                                // Sayýnýn baþýna ve sonuna uygun iþaretler eklenir.
    }
    public void GameStartBtn()
    {
       GameStart = true;                            // Oyunu baþlatmak için kullanýlan bool deðiþkenleri true yapýlýr. Ayrýca buton durumu kapatýlýr.
       StartButton.SetActive(false);
    }
    public void GameRestartBtn()
    {
        RestartBtn.SetActive(false);                // Oyunu yeniden baþlatmak için sahne yeniden yüklenir. Ayrýca buton durumu kapatýlýr.
        SceneManager.LoadScene(0);
    }
    IEnumerator WaitingFuction()
    {
        yield return new WaitForSeconds(0.8f);      // Saða veya Sola dönüþ iþlemleri baþladýðýnda beklenen süre 
        TouchMovement = true;
    }
    IEnumerator ThirdFinishFuntion()            // 3. bitirme çizgisine gelindiðinde gecikmeli olarak bu fonksiyon çalýþýr.
    {
        yield return new WaitForSeconds(2f);
        PlayerMovement = false;         
        PlayerAnim.SetBool("Loop", true);           // Sevinme animasyonu oynatýlýr. Ayrýca toplanan para 3x ile çarpýlarak kasaya aktarýlýr. Oyuna tekrar baþlayabilmek için Restart butonu ekranda belirir.
        FinishMoneyTxt.GetComponent<TextMeshProUGUI>().text = (DollarTotal * 3).ToString();
        RestartBtn.SetActive(true);
    }
}
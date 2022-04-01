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
    float PlayerForwardSpeed = 6f; // Player'�n ileriye do�ru gidi� h�z� de�i�keni
    float PlayerTurnSpeed = 20f; // Player sa�a veya sola a��sal d�n�� h�z�
    float PlayerspeedModifier = 0.01f; // Touch haraketi i�in h�z kat say�s�
    float leftBorder1, rightBorder1, leftBorder2, rightBorder2; // Karakter haraket halindeyken sa�a ve sola ta�malar� �nlemek i�in temas etti�i y�zeyin koordinatlar�n� alarak +2 veya -2 de�erler aras� haraketini sa�lamak.
    float MoneyCounterWaitTime = 0.4f; // UI �zerinde para veya bira toplay�nca ��kan textin ekranda kalma s�resi kontrol�
    float timer = 0.0f; // UI �zerinde para veya bira i�in toplan�ld�ktan sonra timer sayesinde ka� saniye kalabilece�ini kontrol etmek.

    //=====BOOL=====
    bool TurnRight,TurnLeft = false; // Sa�a veya Sola d�n�� i�in trigger objesi sayesinde kontrol sa�lama.
    bool RayCastControl = false; // Bast��� zemin ile Player aras�ndaki mesafeyi kontrol ederek i�lem yapmak i�in kullan�lan bool de�i�keni
    bool GameStart = false; // Oyuna ba�lamak i�in, Start butonuna bas�ld���nda oyundaki fonksiyonlar� ba�latmak i�in kullan�lan bool de�i�keni
    bool MoneyTimer = false; // Dollar veya Biralara �arpt���nda Time.deltatime'� �al��t�rma (+ veya - para toplama yaz�lar�n� ekranda tutulmas�)
    bool TouchMovement = true; // Oyun i�erisinde sa�a ve sola d�n�� oldu�unda touch i�lemini kapatma ve a�mak i�in kullan�l�yor.
    bool PlayerMovement = true; // Player'�n ileriye do�ru haraketi i�in kullan�l�r.

    //=====INT=====
    int collectibleObjectCounter = 0; // Toplanan objelerin say�s�n� tutar.
    int CounterPositiveForUI = 0; // Ekranda Positif nesne toplad���m�zda g�sterilecek olan Textlerin de�erini tutan de�i�ken.
    int CounterNegativeForUI = 0; // Ekranda Negatif nesne toplad���m�zda g�sterilecek olan Textlerin de�erini tutan de�i�ken.
    int DollarTotal = 0; // Ba�lang��taki 20 can� ve daha sonra toplanacak olan objelerin de�erlerini birlikte tutarak ekrandaki level yaz�s�n�n alt�ndaki text verisine bas�lan de�er.
    int LookTo = 0; // 0(ileri) 1(sa�) 2(geri) 3(sol) // Karakterin hangi posisyonda oldu�unu alg�layarak buna g�re i�lem yapt�r�l�r.
    int finishDoorCounter = 0; // 1(poor) 2(avarage) 3(Rich) // Oyun biti� kap�lar�ndaki triggerlar sayesinde oyunu hangi evrede bitirdi�imizi kontrol eden saya�.
    int currentHealth = 20; // Slider i�in ba�lang�� can�.

    //=====STRING=====
    string DollarDefaultNumber = "20"; // Karakter can�n�n ba�lang��taki can�n� 20 olarak belirleyerek. Bundan sonraki toplama i�lemlerini bu de�i�ken ile ger�ekle�ir.
    

    // ===== ANIMATOR =====
    [SerializeField] Animator PlayerAnim; // Karakterin animasyon kontrolc�s�
    [SerializeField] Animator Door1Left, Door1Right, Door2Left, Door2Right, Door3Left, Door3Right; // Biti� �izgisindeki kap�lar�n animasyon kontrolc�leri.

    // ===== GAMEOBJECT =====
    [SerializeField] GameObject StartButton, RestartBtn; // Oyuna ba�lamak i�in kullan�lan button // Oyuna yeniden ba�lamak i�in kullan�lan button.
    [SerializeField] GameObject DollarCounterGameobjectPositive, DollarCounterGameobjectNegative; // Oyun ekran�nda negatif veya positif herhangi bir �ey toplad���m�zda ekrana yeni textler ��kartarak haraket ettirmek i�in kullan�lan textler
    [SerializeField] GameObject PlayerCanvas; // Ekrana ��kart�lacak negatif ve positif textleri Player i�erisindeki Canvas'�n Child objeleri yapmak i�in kullan�lan gameobject.
    [SerializeField] GameObject MoneyPositionPositive,MoneyPositionNegative; // Ekranda nesne toplad���m�zda belirecek olan Negatif ve Positif textlerin spawn olaca�� pozisyonlar.
    [SerializeField] GameObject Poor, Avarage, Rich; // Karakter durumuna g�re karakterin fakir,ortalama veya zengin hallerinin g�r�nt�s�n� a��p kapatmak i�in kullan�lan objeler.
    [SerializeField] GameObject Fill; // Slider'�n i�erisindeki azalan veya artan �ubu�a ula�arak rengini de�i�tirmek i�in kullan�l�r.
    [SerializeField] GameObject MoneyCounterUI; // Level yaz�s�n�n alt�ndaki o anki can�m�z� g�steren counter gameobjectine ula�arak i�erisindeki veriyi almak i�in kullan�l�r.
    [SerializeField] GameObject FinishMoneyTxt; // Oyun bitti�inde ge�ilen kap� say�s�na g�re X(�arp�) bonus alarak kazan�lan dolar miktar�n� ekrana yazd�rmak i�in ula��lan gameobject.

    // ===== OTHER =====
    Touch TheTouch; // Ekran haraketlerini alg�lamak i�in Touch.
    RaycastHit hit; // Karakterin hangi zemin ile temas etti�ini kontrol etmek i�in olu�turulan RaycastHit. Bunun sayesinde karakterin bulundu�u zemindeki s�n�rlar�n� belirleyebiliyoruz.
    public SliderControl sliderControl; // Ekrandaki can slideri 
    [SerializeField] private TextMeshProUGUI DollarCountertxtPositive, DollarCountertxtNegative; // UI'da belirecek olan Text'lere proje alan�ndan ula�ma.
    [SerializeField] TextMeshProUGUI PlayerStatusTxt; // Karakterin o anki durumunu saklad���m�z txt verisi.
    void Start()
    {

    }
    void Update()
    {
        if (GameStart)
        {
            PlayerWalk(); // Player'�n ileriye haraketi
            PlayerTurns(); // Player'�n sa�a veya sola d�n�� yapt�rma i�lemleri
            PlayerTouchControl(); // Player'� haraket ettirme i�lemleri
            PlayerStatusControl(); // Player'�n o anki durumunu kontrol etme i�lemleri
            PlayerAnimationControl(); // Player'�n durumlara g�re animasyonlar�n� �al��t�rma i�lemleri
            PlayerDeadControl(); // Player �ld���nde �al��acak i�lemler.
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
            transform.Translate(Vector3.forward * PlayerForwardSpeed * Time.deltaTime); // Karakterin ileriye do�ru haraketini sa�lar.
        }
    }
    void PlayerDeadControl()
    {
        if (int.Parse(MoneyCounterUI.GetComponent<TextMeshProUGUI>().text) <= 0) // Player'�n can� 0'a indi�inde veya daha d���k oldu�unda sahneyi yeniden y�kler.
        {
            SceneManager.LoadScene(0);
        }
    }
    void PlayerTurns()
    {
        // LookTo 0(ileri) 1(sa�) 2(geri) 3(sol)
        if (TurnRight) // Karakter sa�a d�n�� alan�na gelince Trigger sayesinde �al���r.
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), PlayerTurnSpeed * Time.deltaTime); // Player'�n Y rotasyonunu 90 dereceye belirlenen zaman �arpan� boyunca �evirir.
            if (transform.localRotation.eulerAngles.y >= 89f) // Player'� zaman �arpan� ile �evirdi�imizden d�n�� eksik veya fazla olmas� durumunda Player rotasyonu 90'a e�itlenir.
            {
                TurnRight = false;
                transform.rotation = Quaternion.Euler(transform.rotation.x, 90f, transform.rotation.z);
                LookTo +=1;
                if (LookTo < 0)
                {
                    LookTo += 4;                         // LookTo de�i�keni ile olu�turulan fonksiyon sayesinde karakterin hangi posisyonda oldu�unu tespit ederek hangi Koordinattaki Touch verilerinin �al��aca�� belirlenir.
                }
                else if (LookTo >= 4)
                {
                    LookTo %= 4;
                }
            }
        }
        if (TurnLeft) // Karakter sola d�n�� alan�na gelince Trigger sayesinde �al���r.
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), PlayerTurnSpeed * Time.deltaTime); // Player'�n Y rotasyonunu 0 dereceye belirlenen zaman �arpan� boyunca �evirir.
            if (transform.localRotation.eulerAngles.y <= 1f)
            {
                TurnLeft = false;
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z); // Player'� zaman �arpan� ile �evirdi�imizden d�n�� eksik veya fazla olmas� durumunda Player rotasyonu 90'a e�itlenir.
                LookTo -= 1;
                if (LookTo < 0)
                {
                    LookTo += 4;                         // LookTo de�i�keni ile  olu�turulan fonksiyon sayesinde karakterin hangi posisyonda oldu�unu tespit ederek hangi Koordinattaki Touch verilerinin �al��aca�� belirlenir.
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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10.0f)) // Karakter'den  yere do�ru 10 b�y�kl���nde �izgi g�nderilir.
        {
            RayCastControl = true; // Bu �izgi g�nderilerek Player hangi zemin ile temas ediyorsa onun transform verilerine ula�arak Player'�n haraket edece�i konumlar olu�turulur.
        }     
        if (Input.touchCount > 0)
        {
            TheTouch = Input.GetTouch(0);                       // Ekranda Dokunma haraketleri oldu�unda �al��an i�lemler.
            if (TheTouch.phase == TouchPhase.Moved)
            {
                // LookTo 0(ileri) 1(sa�) 2(geri) 3(sol)
                if (LookTo == 0 && TouchMovement == true)       // LookTo sayesinde hangi posisyonda oldu�umuzu belirleyerek buna uygun haraket i�lemleri uygulan�r.
                {
                    leftBorder1 = hit.collider.transform.position.x + 2f;      //Raycast sayesinde Player'�n temas etti�i zeminin Transform bilgilerine ula�arak o zemin �zerinde karakter maksimum/minimum +2 veya -2 b�y�kl�kte haraket sa�lar.
                    rightBorder1 = hit.collider.transform.position.x - 2f;
                    if (RayCastControl && transform.position.x <= leftBorder1 && transform.position.x >= rightBorder1)
                    {
                        transform.position = new Vector3(transform.position.x + TheTouch.deltaPosition.x * PlayerspeedModifier, transform.position.y, transform.position.z); // Karakterin sa�a veya sola haraket i�lemleri
                    }
                    if (transform.position.x > leftBorder1)
                    {
                        transform.position = new Vector3(leftBorder1, transform.position.y, transform.position.z); // Herhangi bir �ekilde maksimum veya minimum haraket alanlar� a��l�rsa minimum veya maksimum haraket alanlar�na geri d�n�l�r.
                    }
                    if (transform.position.x < rightBorder1)
                    {
                        transform.position = new Vector3(rightBorder1, transform.position.y, transform.position.z);
                    }
                }
                if (LookTo == 1 && TouchMovement == true)
                {
                    leftBorder2 = hit.collider.transform.position.z + 2f;      //Raycast sayesinde Player'�n temas etti�i zeminin Transform bilgilerine ula�arak o zemin �zerinde karakter maksimum/minimum +2 veya -2 b�y�kl�kte haraket sa�lar.
                    rightBorder2 = hit.collider.transform.position.z - 2f;
                    if (RayCastControl && transform.position.z <= leftBorder2 && transform.position.z >= rightBorder2)
                    { 
                        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - TheTouch.deltaPosition.x * PlayerspeedModifier); // Karakterin sa�a veya sola haraket i�lemleri
                    }
                    if (transform.position.z > leftBorder2)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y, leftBorder2); // Herhangi bir �ekilde maksimum veya minimum haraket alanlar� a��l�rsa minimum veya maksimum haraket alanlar�na geri d�n�l�r.
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
        PlayerAnim.SetBool("�dleToPoor", true); // Karakterin ba�lang��taki idle animasyonu aktif etmek i�in kullan�l�r.
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "rotateRightTrigger") // Sa�a do�ru d�n��� kontrol etmek i�in kullan�lan Trigger Objesi
        {
            TouchMovement = false;
            TurnRight = true;
            StartCoroutine(WaitingFuction()); // Belirli s�re beklenerek yap�lan i�lemler i�in kullan�lan fonksiyon.
        }
        if (other.gameObject.tag == "rotateLeftTrigger") // Sola do�ru d�n��� kontrol etmek i�in kullan�lan Trigger Objesi
        {
            TouchMovement = false;
            TurnLeft = true;
            StartCoroutine(WaitingFuction());   // Belirli s�re beklenerek yap�lan i�lemler i�in kullan�lan fonksiyon.
        }
        if(other.gameObject.tag == "dollar") // Dollar nesnesini toplad���n� kontrol etmek i�in kullan�lan Trigger Objesi
        {
            TakeHeal(2);
            collectibleObjectCounter+=2;
            CounterPositiveForUI += 2;                  // �arp�lan nesne dolar ise de�er 2 artar ve kendini imha eder. Ayr�ca WallTriggerPositive fonksiyonu �al���r.
            Destroy(other.gameObject);
            WallTriggerPositive();
        }
        else if(other.gameObject.tag == "beer") // Bira nesnesini toplad���n� kontrol etmek i�in kullan�lan Trigger Objesi
        {
            TakeDamage(2);
            collectibleObjectCounter-= 2;               // �arp�lan nesne beer ise de�er 2 azal�r ve kendini imha eder. Ayr�ca WallTriggerPositive fonksiyonu �al���r.
            CounterNegativeForUI -= 2; 
            Destroy(other.gameObject);
            WallTriggerNegative();
        }
        else if(other.gameObject.tag == "GreenTrigger")  // Ye�il alan i�erisinden ge�erek +20 puan ald���n� kontrol etmek i�in kullan�lan Trigger Objesi
        {
            TakeHeal(20);
            collectibleObjectCounter+= 20;
            CounterPositiveForUI += 20;
            Destroy(other.gameObject);                  // �arp�lan nesne Green alan ise de�er 20 artar ve kendini imha eder. Ayr�ca WallTriggerPositive fonksiyonu �al���r.
            WallTriggerPositive();
            CounterPositiveForUI = 0;
        }
        else if(other.gameObject.tag == "RedTrigger") // K�rm�z� alan i�erisinden ge�erek -20 puan ald���n� kontrol etmek i�in kullan�lan Trigger Objesi
        {
            TakeDamage(20);
            collectibleObjectCounter-= 20;
            CounterNegativeForUI -= 20;
            Destroy(other.gameObject);                  // �arp�lan nesne Red alan ise de�er 20 azal�r ve kendini imha eder. Ayr�ca WallTriggerPositive fonksiyonu �al���r.
            WallTriggerNegative();
            CounterNegativeForUI = 0;
        }
        if (other.gameObject.tag != "beer" || other.gameObject.tag != "dollar" || other.gameObject.tag != "RedTrigger" || other.gameObject.tag != "GreenTrigger")
        {
            if (timer > MoneyCounterWaitTime)
            {
                CounterPositiveForUI = 0;
                CounterNegativeForUI = 0;                   // Timer ile birlikte toplanan dolarlar�n ekranda stacklenece�i s�resini ayarlama i�lemleri ve belirli s�renin �zerinde stacklenmenin tekrar s�f�rlanmas�.
                MoneyTimer = false;
                timer = 0.0f;
            }
        }
        if (other.gameObject.tag == "FinishDoor") // Biti� �izgisinde bulunan 3 farkl� Trigger sayesinde Player'�n o anki can�na veya dolar�na g�re oyunu hangi seviyede bitirece�ini belirleyerek i�lemler yapt�r�l�r.
        {
            finishDoorCounter++; // 1(poor) 2(avarage) 3(Rich) // 3 farkl� biti� �izgisindeki Triggerlara �arparak counter say�s� kontrol ettirilir. Buna g�re hangi seviyede oldu�u tespit edilir.
            TouchMovement = false;
            Destroy(other.gameObject);
            if (sliderControl.slider.value <= 40 && finishDoorCounter ==1)
            {
                // �z�lme animasyonu oynat�lacak.
                // Para 1x olarak aktar�lacak.
                // Kap� a��lmayacak.
                PlayerMovement = false; // Player haraketi engellenir.
                PlayerAnim.SetBool("Loop", true); // �z�lme animasyonu de�i�keni True yap�larak �z�lme animasyonu oynat�l�r.
                FinishMoneyTxt.GetComponent<TextMeshProUGUI>().text = (DollarTotal).ToString(); // Oyun sonunda toplad���m�z para 1x ile �arp�l�r kasaya aktar�l�r.
                RestartBtn.SetActive(true); // Tek levelden olu�tu�u i�in level ba�lang�c�na d�nmek i�in Restart butonu kullan�l�r. Bu buton aktif hale getirilir.
            } 
            else if(sliderControl.slider.value > 40 && finishDoorCounter == 1)
            {
                // 1.Kap� a��lacak.
                Door1Left.SetInteger("New Int", 1);                 // 1. kap� i�erisindeki animasyon de�erleri artt�r�larak animasyonun oynat�lmas� sa�lan�r.
                Door1Right.SetInteger("New Int", 1);
            }
            if(sliderControl.slider.value <= 70 && finishDoorCounter == 2)
            {
                PlayerMovement = false; // Player haraketi engellenir.
                // 2.Kap� a��lmad� ve oyun bitti.
                PlayerAnim.SetBool("Loop", true);   // Sevinme animasyonu de�i�keni True yap�larak �z�lme animasyonu oynat�l�r.
                FinishMoneyTxt.GetComponent<TextMeshProUGUI>().text = (DollarTotal*2).ToString();   // Oyun sonunda toplad���m�z para 2x ile �arp�l�r kasaya aktar�l�r.
                RestartBtn.SetActive(true);
            }
            else if (sliderControl.slider.value > 70 && finishDoorCounter == 2)
            {
                // 2. Kap� a��ld�.
                Door2Left.SetInteger("New Int", 1);                 // 2. kap� i�erisindeki animasyon de�erleri artt�r�larak animasyonun oynat�lmas� sa�lan�r.
                Door2Right.SetInteger("New Int", 1);
            }
            if (sliderControl.slider.value > 70 && finishDoorCounter == 3)
            {
                // 3. Kap� a��ld� ve oyun bitti.
                Door3Left.SetInteger("New Int", 1);
                Door3Right.SetInteger("New Int", 1);            // 3. kap� i�erisindeki animasyon de�erleri artt�r�larak animasyonun oynat�lmas� sa�lan�r.
                StartCoroutine(ThirdFinishFuntion());            // 3. kap� a��ld�ktan belirli s�re sonra �al��acak fonksiyon.
            }
        }
    }
    void TakeDamage(int damage)     // Hasar ald���m�zda slidera veri g�ndermezi sa�layan fonksiyon.
    {
        currentHealth -= damage;
        sliderControl.SetHealth(currentHealth);
    }
    void TakeHeal(int damage)       // �yile�tirme ald���m�zda slidera veri g�ndermezi sa�layan fonksiyon.
    {
        currentHealth += damage;
        sliderControl.SetHealth(currentHealth);
    }
    void PlayerStatusControl()
    {
        if (sliderControl.slider.value <= 40f)          // Karakterin slider value de�erine g�re slider renginin ayarlanmas�, karakter g�r�n�mleri aktifle�tirilmesi, karakter durum yaz�lar� i�lemleri 
        {
            PlayerStatusTxt.text = "Poor";
            Poor.SetActive(true);
            Avarage.SetActive(false);
            Rich.SetActive(false);
            PlayerStatusTxt.color = Color.red;
            Fill.GetComponent<Image>().color = Color.red;
        }
        else if (sliderControl.slider.value <= 70f)     // Karakterin slider value de�erine g�re slider renginin ayarlanmas�, karakter g�r�n�mleri aktifle�tirilmesi, karakter durum yaz�lar� i�lemleri 
        {
            PlayerStatusTxt.text = "Average";
            Poor.SetActive(false);
            Avarage.SetActive(true);
            Rich.SetActive(false);
            PlayerStatusTxt.color = new Color32(239, 148, 15, 255);
            Fill.GetComponent<Image>().color = new Color32(239, 148, 15, 255);
        }
        else
        {                                               // Karakterin slider value de�erine g�re slider renginin ayarlanmas�, karakter g�r�n�mleri aktifle�tirilmesi, karakter durum yaz�lar� i�lemleri 
            Poor.SetActive(false);
            Avarage.SetActive(false);
            Rich.SetActive(true);
            PlayerStatusTxt.text = "Rich";
            PlayerStatusTxt.color = Color.green;
            Fill.GetComponent<Image>().color = Color.green; 
        }
    }
    void WallTriggerPositive() // Bu fonksiyon Positif bir nesneye �arp�ld���nda aktif olarak �al���r. Genel amac� toplanan paralar�n miktar�n� ekranda g�stermektir.
    {
        DollarTotal = (int.Parse(DollarDefaultNumber) + (collectibleObjectCounter)); // Ba�lang�� can� olan 20 ile birlikte toplanan nesneyi de�i�ken i�erisinde tutarak ekrana yazd�rma i�lemini yapt�r�r�z.
        DollarCountertxtPositive.text = DollarTotal.ToString(); // Nesneleri toplad���m�zda ekranda bir ka� saniye beliren textlerin i�erine de�er yazar�z.
        MoneyCounterUI.GetComponent<TextMeshProUGUI>().text = DollarTotal.ToString(); // Ekrandaki level yaz�s�n alt�ndaki para de�erini yazar.
        MoneyTimer = true;  // Bu de�er True konuma ge�ti�inde bir Timer harakete ge�er ve belirli s�re boyunca oyun i�erisindeki paralar stacklan�r. Belirli zaman sonra toplama olmaz ise s�f�rlan�r.
        PlayerAnim.SetInteger("Dcounter", DollarTotal); // Player i�erisindeki sevinme veya �z�lme animasyonlar� DollarTotal'den al�nan verilere g�re �al���r.

        var myNewSmoke = Instantiate(DollarCounterGameobjectPositive, MoneyPositionPositive.transform.position, Quaternion.identity); // Yeni bir Pozitif Txt gameobjecti olu�turulur. 
        myNewSmoke.transform.SetParent(PlayerCanvas.gameObject.transform);                                                            // Player i�erisindeki Canvas alt�nda olmas� sa�lan�r.
        myNewSmoke.transform.localScale = new Vector3(1f, 1f, 1f);                                                                    // Boyutu belirlenir.
        myNewSmoke.GetComponent<RectTransform>().DOLocalMoveY(10f, 0.5f).OnComplete(() => Destroy(myNewSmoke));                       // Pozitif olan txt dosyas� belirlenen konuma yumu�ak �ekilde haraket eder ve kaybolur.

        myNewSmoke.GetComponent<TextMeshProUGUI>().text = "+ " + CounterPositiveForUI.ToString() + " $";                              // Say�n�n ba��na ve sonuna uygun i�aretler eklenir.
    }
    void WallTriggerNegative()  // Bu fonksiyon Negatif bir nesneye �arp�ld���nda aktif olarak �al���r. Genel amac� toplanan paralar�n miktar�n� ekranda g�stermektir.
    {
        DollarTotal = (int.Parse(DollarDefaultNumber) + (collectibleObjectCounter));
        DollarCountertxtNegative.text = DollarTotal.ToString();
        MoneyCounterUI.GetComponent<TextMeshProUGUI>().text = DollarTotal.ToString();
        MoneyTimer = true;
        PlayerAnim.SetInteger("Dcounter", DollarTotal);

        var myNewSmoke = Instantiate(DollarCounterGameobjectNegative, MoneyPositionNegative.transform.position, Quaternion.identity);   // Yeni bir Negatif Txt gameobjecti olu�turulur. 
        myNewSmoke.transform.SetParent(PlayerCanvas.gameObject.transform);                                                              // Player i�erisindeki Canvas alt�nda olmas� sa�lan�r.
        myNewSmoke.transform.localScale = new Vector3(1f, 1f, 1f);                                                                      // Boyutu belirlenir.
        myNewSmoke.GetComponent<RectTransform>().DOLocalMoveY(10f, 0.5f).OnComplete(() => Destroy(myNewSmoke));                         // Negatif olan txt dosyas� belirlenen konuma yumu�ak �ekilde haraket eder ve kaybolur.

        myNewSmoke.GetComponent<TextMeshProUGUI>().text = "- " + CounterNegativeForUI.ToString() + " $";                                // Say�n�n ba��na ve sonuna uygun i�aretler eklenir.
    }
    public void GameStartBtn()
    {
       GameStart = true;                            // Oyunu ba�latmak i�in kullan�lan bool de�i�kenleri true yap�l�r. Ayr�ca buton durumu kapat�l�r.
       StartButton.SetActive(false);
    }
    public void GameRestartBtn()
    {
        RestartBtn.SetActive(false);                // Oyunu yeniden ba�latmak i�in sahne yeniden y�klenir. Ayr�ca buton durumu kapat�l�r.
        SceneManager.LoadScene(0);
    }
    IEnumerator WaitingFuction()
    {
        yield return new WaitForSeconds(0.8f);      // Sa�a veya Sola d�n�� i�lemleri ba�lad���nda beklenen s�re 
        TouchMovement = true;
    }
    IEnumerator ThirdFinishFuntion()            // 3. bitirme �izgisine gelindi�inde gecikmeli olarak bu fonksiyon �al���r.
    {
        yield return new WaitForSeconds(2f);
        PlayerMovement = false;         
        PlayerAnim.SetBool("Loop", true);           // Sevinme animasyonu oynat�l�r. Ayr�ca toplanan para 3x ile �arp�larak kasaya aktar�l�r. Oyuna tekrar ba�layabilmek i�in Restart butonu ekranda belirir.
        FinishMoneyTxt.GetComponent<TextMeshProUGUI>().text = (DollarTotal * 3).ToString();
        RestartBtn.SetActive(true);
    }
}
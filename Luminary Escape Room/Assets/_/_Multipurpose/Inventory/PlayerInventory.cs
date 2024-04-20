using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=HGol5qhqjOE&ab_channel=FreedomCoding
//https://www.youtube.com/watch?v=Josw0x2geuQ&ab_channel=FreedomCoding
//https://forum.unity.com/threads/how-to-get-the-button-gameobject-when-it-is-clicked.447617/

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]

    public List<itemType> inventoryList;
    public List<ItemScriptableObject> inventoryInfoList;
    public List<GameObject> allItems; //item prefabs go in here
    public int selectedItem;
    public GameObject currentItem;
    public GameObject lastItem;
    public string currentItemName;
    public string lastItemName;

    public GameObject powder;
    public Transform powderSpawn;
    public bool phoenixDead;
    public GameObject phoenixAsh;
    public Transform spawnAsh;
    public bool makeAlcohol;
    public bool makeCrystals;

        private static PlayerInventory instance;

    [Space(20)]
    [Header("Item Info UI")]
    [SerializeField] private RectTransform itemInfoUI;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    //public Sprite mash, fermentedMash, emptyJar, alcohol, vinegarSolution, evaporatedSolution, mixedSolution, emptyBottle, crystals;

    [Space(20)]
    [Header("UI")]
    [SerializeField] private Transform itemSpritesParent; //parent object for the item sprites
    [SerializeField] private Transform itemSelectedIndicatorsParent; //parent object for the selection indicator
    [SerializeField] private List<Image> itemSprites; //list of item sprite children
    [SerializeField] private List<Image> itemSelectedIndicators; //list of selection indicator children
    [SerializeField] Sprite emptySlotSprite;

    [Space(20)]

    [SerializeField] private Camera mainCamera;
    [SerializeField] SocketWork server;
    [SerializeField] GameObject[] itemList;

    public GameObject fire;
    public GameObject rightFire;

    public Material blue;
    public Material green;

    float updatedCount = 0;
    
    private Dictionary<itemType, GameObject> itemSetActive = new Dictionary<itemType, GameObject>() { };

    private readonly Queue<Action> actionQueue = new Queue<Action>();


    public static void Enqueue(Action action)
    {
        lock (instance.actionQueue)
        {
            instance.actionQueue.Enqueue(action);
        }
    }

    void Awake()
    {
        itemInfoUI.gameObject.SetActive(false);
        phoenixDead = false;
        makeAlcohol = false;
        makeCrystals = false;
        instance=this;

        for (int x = 0; x<allItems.Count; x++){
            Debug.Log(allItems[x].name);
            Debug.Log(allItems[x].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_type);
            Debug.Log(allItems[x].name);
            itemSetActive.Add(allItems[x].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_type, allItems[x]);
        }
        

        //grabs the inventory images without having to drag them into the inspector, scalable
        foreach (Transform child in itemSpritesParent.transform)
        {
            itemSprites.Add(child.gameObject.GetComponent<Image>());
        }
        foreach (Transform child in itemSelectedIndicatorsParent.transform)
        {
            itemSelectedIndicators.Add(child.gameObject.GetComponent<Image>());
        }
        //UpdateInventoryUI();
    }

    void Update()
    {
        //raycast when clicked
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            //add object to inventory if it has the ItemPickable script component and it will be destroyed
            if (Physics.Raycast(ray, out hitInfo))
            {
                if(hitInfo.collider.GetComponent<ItemPickable>() != null)
                {
                    IPickable item = hitInfo.collider.GetComponent<IPickable>();
                    ItemPickable itemInfo = hitInfo.collider.GetComponent<ItemPickable>();
                    if (item != null)
                    {
                        //inventoryList.Add(itemInfo.itemScriptableObject.item_type);
                        server.addItem(itemInfo.itemScriptableObject.id);

                        item.PickItem();
                        Debug.Log("got here");
                     

                        UpdateInventoryUI();
                    }
                }
                if(hitInfo.collider.GetComponent<WorldItem>() != null)
                {
                    WorldItemInteractions(hitInfo.collider.GetComponent<WorldItem>().itemName);
                }
            }
        }
    }

    
    void WorldItemInteractions(string worldItemName)
    {
        //Blue Wall interactions
        if (currentItemName == "Fruit" && worldItemName == "Mortar")
        {
            server.addItem(9);
            server.removeItem(4);
            currentItemName = null;
        }
        if (currentItemName == "Fermented" && worldItemName == "Distillery" && !makeCrystals)
        {
            server.addItem(11);
            server.removeItem(10);
            makeAlcohol = true;
            makeCrystals = false;
            currentItemName = null;
        }
        if (currentItemName == "Mixed Solution" && worldItemName == "Distillery" && !makeAlcohol)
        {
            Debug.Log("bottle");
             server.addItem(11);
             server.removeItem(14);
             makeCrystals = true;
             makeAlcohol = false;
            currentItemName = null;
        }
        if (currentItemName == "Fire" && worldItemName == "Distillery")
        {
            Debug.Log("heat distillery");
            if(makeAlcohol == true)
            {
                Debug.Log("alcohol");
                server.addItem(12);
                server.removeItem(11);
                makeAlcohol = false;
                currentItemName = null;
            }
            else if (makeCrystals == true)
            {
                Debug.Log("crystals");
                server.addItem(15);
                makeCrystals = false;
                currentItemName = null;
            }
        }
        if (currentItemName == "Alcohol" && worldItemName == "Blue")
        {
            Debug.Log("blue flame lighting");

             fire.GetComponent<MeshRenderer> ().material = blue;
             server.winBlue();
            //code for lighting flame
        }


        //Green Wall interactions
        if (currentItemName == "Key" && worldItemName == "Chest")
        {
            Debug.Log("powder falls on ground");
            currentItemName = null;
            //Instantiate(powder, powderSpawn);
            server.addItem(5);
            server.removeItem(1);
        }
        if (currentItemName == "Crystals" && worldItemName == "Green")
        {
             rightFire.GetComponent<MeshRenderer> ().material = green;
             server.winGreen();
            //code for lighting flame
        }
        UpdateInventoryUI();
    }

    void CombineItems()
    {
        //Blue Wall combinations
        if (currentItemName == "Mash" && lastItemName == "Time" || currentItemName == "Time" && lastItemName == "Mash")
        {
            Debug.Log("fermented mash");
            server.removeItem(9);
            server.addItem(10);
            Debug.Log("kill phoenix");

            server.killPhoenix();
            

        }

        //Green Wall combinations
        if (currentItemName == "Vinegar" && lastItemName == "Powder" || currentItemName == "Powder" && lastItemName == "Vinegar")
        {
            server.removeItem(0);
            server.removeItem(5);
            server.addItem(7);
            Debug.Log("vinegar solution");

        }
        if (currentItemName == "Vinegar Solution" && lastItemName == "Time" || currentItemName == "Time" && lastItemName == "Vinegar Solution")
        {
            Debug.Log("evaporated solution");
            server.killPhoenix();
            server.removeItem(7);
            server.addItem(13);
            //remove solution add evaporated
        }
        if (currentItemName == "Evaporated Solution" && lastItemName == "Ash" || currentItemName == "Ash" && lastItemName == "Evaporated Solution")
        {
            Debug.Log("mixed solution");
            server.removeItem(6);
            server.removeItem(13);
            server.addItem(14);

            //remove evaporated add mixed
        }
    }

    public void SpawnPhoenixAsh()
    {
        if (phoenixDead == false)
        {
            phoenixDead = true;
            Instantiate(phoenixAsh, spawnAsh);
        }
    }

    public void SlotClicked()
    {
        Debug.Log("clicked");
        lastItem = currentItem;
        lastItemName = currentItemName;
        selectedItem = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        
        UpdateInventoryUI();
        currentItem = EventSystem.current.currentSelectedGameObject;
        Debug.Log(currentItem.GetComponent<Image>().sprite.name);

        currentItemName = currentItem.GetComponent<Image>().sprite.name;
        CombineItems();

        //display item info
        if(currentItem == lastItem)
        {
            itemInfoUI.gameObject.SetActive(true);
            //itemInfoUI.anchoredPosition = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition;
            itemNameText.text = currentItemName;
            for (int x = 0; x < itemList.Length; x++)
            {
                if (itemList[x].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_sprite.name == currentItemName)
                {
                    itemDescriptionText.text = itemList[x].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_description;
                }
            }
        }
        else if (currentItem != lastItem)
        {
            itemInfoUI.gameObject.SetActive(false);
        }
    }

    public void CloseInfoButton()
    {
        itemInfoUI.gameObject.SetActive(false);
    }

    ItemPickable FindItem(int id)
    {
        for (int x = 0; x < itemList.Length; x++)
        {
            if (itemList[x].GetComponentInChildren<ItemPickable>().itemScriptableObject.id == id)
            {
                return itemList[x].GetComponentInChildren<ItemPickable>();
            }
        }
        return null;
    }

    public void UpdateInventoryUI()
    {
        
        //.Log("inv"+inventoryList.Count);
        for (int i = 0; i < itemSprites.Count; i++)
        {
            Debug.Log("updated" + updatedCount);
            if (i < updatedCount && itemSprites[i].sprite == null)
            {
                Debug.Log("first" +itemSetActive.Count + itemSetActive[inventoryList[i]]);
                itemSprites[i].sprite = itemSetActive[inventoryList[i]].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_sprite;
                itemSprites[i].enabled = true;
            }
            else if (i < updatedCount && itemSprites[i].sprite != null)
            {
                Debug.Log("second");

                itemSprites[i].enabled = true;
            }
            else
            {
                Debug.Log("third "+updatedCount);

                itemSprites[i].sprite = null;
                itemSprites[i].enabled = false;
            }
        }

        if (updatedCount > 0)
        {
            int a = 0;
            foreach (Image image in itemSelectedIndicators)
            {
                if (a == selectedItem)
                {
                    image.gameObject.SetActive(true);
                }
                else
                {
                    image.gameObject.SetActive(false);
                }
                a++;
            }
        }
         Debug.Log("Count"+actionQueue.Count);
        if(actionQueue.Count>0){
                    Debug.Log("invoking");

        actionQueue.Dequeue().Invoke();
        }
    }

public void UpdateInventoryUI(string[] items)
    {
        inventoryList = new List<itemType>();
        for(int x = 0; x<items.Length; x++){
           Debug.Log("Found item" + items.Length+ items[x]);

            inventoryList.Add(FindItem(int.Parse(items[x])).itemScriptableObject.item_type);

        }
        updatedCount = items.Length;
        //.Log("inv"+inventoryList.Count);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("test" + items.Length);
            if (i < items.Length)
            {
                Debug.Log(i+"got inside"+items.Length);
                itemSprites[i].sprite = FindItem(int.Parse(items[i])).itemScriptableObject.item_sprite;
                itemSprites[i].enabled = true;
            }
            else
            {
                Debug.Log("null");
                itemSprites[i].sprite = null;
                itemSprites[i].enabled = false;
            }
        }

        if (updatedCount > 0)
        {
            int a = 0;
            foreach (Image image in itemSelectedIndicators)
            {
                if (a == selectedItem)
                {
                    image.gameObject.SetActive(true);
                }
                else
                {
                    image.gameObject.SetActive(false);
                }
                a++;
            }
        }
        Debug.Log("CountServer"+actionQueue.Count);
                if(actionQueue.Count>0){
                    Debug.Log("invoking server");
        actionQueue.Dequeue().Invoke();
       }
    }
    
}



public interface IPickable
{
    void PickItem();
}
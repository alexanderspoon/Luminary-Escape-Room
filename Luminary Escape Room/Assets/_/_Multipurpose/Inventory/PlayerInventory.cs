using System.Collections;
using System.Collections.Generic;
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

    public Sprite mash, fermentedMash, emptyJar, alcohol, vinegarSolution, evaporatedSolution, mixedSolution, emptyBottle, crystals;

    [Space(20)]
    [Header("UI")]
    [SerializeField] private Transform itemSpritesParent; //parent object for the item sprites
    [SerializeField] private Transform itemSelectedIndicatorsParent; //parent object for the selection indicator
    [SerializeField] private List<Image> itemSprites; //list of item sprite children
    [SerializeField] private List<Image> itemSelectedIndicators; //list of selection indicator children
    [SerializeField] Sprite emptySlotSprite;

    [SerializeField] private Camera mainCamera;
    [SerializeField] SocketWork server;
    [SerializeField] GameObject[] itemList;

    private Dictionary<itemType, GameObject> itemSetActive = new Dictionary<itemType, GameObject>() { };

    void Awake()
    {
        phoenixDead = false;
        makeAlcohol = false;
        makeCrystals = false;

        for (int x = 0; x<allItems.Count; x++){
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
                        inventoryList.Add(itemInfo.itemScriptableObject.item_type);
                        // server.addItem(itemInfo.itemScriptableObject.id);

                        item.PickItem();
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

    //i couldn't think of a better way so i brute forced it
    void WorldItemInteractions(string worldItemName)
    {
        //Blue Wall interactions
        if (currentItemName == "Fruit" && worldItemName == "Mortar")
        {
            Debug.Log("mash");
            currentItem.GetComponent<Image>().sprite = mash;
        }
        if (currentItemName == "Fermented Mash" && worldItemName == "Distillery")
        {
            Debug.Log("jar");
            currentItem.GetComponent<Image>().sprite = emptyJar;
            makeAlcohol = true;
            makeCrystals = false;
        }
        if (currentItemName == "Mixed Solution" && worldItemName == "Distillery")
        {
            Debug.Log("bottle");
            currentItem.GetComponent<Image>().sprite = emptyBottle;
            makeCrystals = true;
            makeAlcohol = false;
        }
        if (currentItemName == "Fire" && worldItemName == "Distillery")
        {
            Debug.Log("heat distillery");
            if(makeAlcohol == true)
            {
                Debug.Log("alcohol");
                foreach (Transform child in itemSpritesParent.transform)
                {
                    if (child.GetComponent<Image>().sprite == emptyJar)
                    {
                        child.GetComponent<Image>().sprite = alcohol;
                    }
                }
                makeAlcohol = false;
            }
            else if (makeCrystals == true)
            {
                Debug.Log("crystals");
                foreach (Transform child in itemSpritesParent.transform)
                {
                    if (child.GetComponent<Image>().sprite == emptyBottle)
                    {
                        child.GetComponent<Image>().sprite = crystals;
                    }
                }
                makeCrystals = false;
            }
        }
        if (currentItemName == "Alcohol" && worldItemName == "Blue")
        {
            Debug.Log("blue flame lighting");
            //code for lighting flame
        }


        //Green Wall interactions
        if (currentItemName == "Key" && worldItemName == "Chest")
        {
            Debug.Log("powder falls on ground");
            selectedItem = 0;
            currentItem.gameObject.SetActive(false);
            Instantiate(powder, powderSpawn);
        }
        if (currentItemName == "Crystals" && worldItemName == "Green")
        {
            Debug.Log("green flame lighting");
            //code for lighting flame
        }
    }

    void CombineItems()
    {
        //Blue Wall combinations
        if (currentItemName == "Mash" && lastItemName == "Time" || currentItemName == "Time" && lastItemName == "Mash")
        {
            Debug.Log("fermented mash");
            SpawnPhoenixAsh();
            if (currentItemName == "Mash")
            {
                currentItem.GetComponent<Image>().sprite = fermentedMash;
            }
            else if (currentItemName == "Time")
            {
                lastItem.GetComponent<Image>().sprite = fermentedMash;
            }
        }

        //Green Wall combinations
        if (currentItemName == "Vinegar" && lastItemName == "Powder" || currentItemName == "Powder" && lastItemName == "Vinegar")
        {
            Debug.Log("vinegar solution");
            if(currentItemName == "Vinegar")
            {
                currentItem.GetComponent<Image>().sprite = vinegarSolution;
                //destroy last item
                lastItem.gameObject.SetActive(false);
            }
            else if (currentItemName == "Powder")
            {
                lastItem.GetComponent<Image>().sprite = vinegarSolution;
                //destroy current item
                currentItem.gameObject.SetActive(false);
            }
        }
        if (currentItemName == "Vinegar Solution" && lastItemName == "Time" || currentItemName == "Time" && lastItemName == "Vinegar Solution")
        {
            Debug.Log("evaporated solution");
            SpawnPhoenixAsh();
            if (currentItemName == "Vinegar Solution")
            {
                currentItem.GetComponent<Image>().sprite = evaporatedSolution;
            }
            if (currentItemName == "Time")
            {
                lastItem.GetComponent<Image>().sprite = evaporatedSolution;
            }
        }
        if (currentItemName == "Evaporated Solution" && lastItemName == "Ash" || currentItemName == "Ash" && lastItemName == "Evaporated Solution")
        {
            Debug.Log("mixed solution");
            if (currentItemName == "Evaporated Solution")
            {
                currentItem.GetComponent<Image>().sprite = mixedSolution;
                //destroy last item
                lastItem.gameObject.SetActive(false);
            }
            if (currentItemName == "Ash")
            {
                lastItem.GetComponent<Image>().sprite = mixedSolution;
                //destroy current item
                currentItem.gameObject.SetActive(false);
            }
        }
    }

    void SpawnPhoenixAsh()
    {
        if (phoenixDead == false)
        {
            phoenixDead = true;
            Instantiate(phoenixAsh, spawnAsh);
        }
    }

    public void SlotClicked()
    {
        lastItem = currentItem;
        lastItemName = currentItemName;
        selectedItem = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        UpdateInventoryUI();

        currentItem = EventSystem.current.currentSelectedGameObject;
        currentItemName = currentItem.GetComponent<Image>().sprite.name;
        CombineItems();
    }

    ItemPickable FindItem(int id)
    {
        for (int x = 0; x < itemList.Length; x++)
        {
            if (itemList[x].GetComponent<ItemPickable>().itemScriptableObject.id == id)
            {
                return itemList[x].GetComponent<ItemPickable>();
            }
        }
        return null;
    }

    public void UpdateInventoryUI()
    {
        //.Log("inv"+inventoryList.Count);
        for (int i = 0; i < itemSprites.Count; i++)
        {
            if (i < inventoryList.Count && itemSprites[i].sprite == null)
            {
                itemSprites[i].sprite = itemSetActive[inventoryList[i]].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_sprite;
                itemSprites[i].enabled = true;
            }
            else if (i < inventoryList.Count && itemSprites[i].sprite != null)
            {
                itemSprites[i].enabled = true;
            }
            else
            {
                itemSprites[i].sprite = emptySlotSprite;
                itemSprites[i].enabled = false;
            }
        }

        if (inventoryList.Count > 0)
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
    }

        public void UpdateInventoryUI(string[] items)
    {
        //.Log("inv"+inventoryList.Count);
        for (int i = 0; i < items.Length; i++)
        {
            Debug.Log("test");
            if (i < items.Length)
            {
                Debug.Log("got inside"+items.Length);
                itemSprites[i].sprite = FindItem(int.Parse(items[i])).itemScriptableObject.item_sprite;
                itemSprites[i].enabled = true;
            }
            else
            {
                itemSprites[i].sprite = emptySlotSprite;
                itemSprites[i].enabled = false;
            }
        }

        if (inventoryList.Count > 0)
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
    }
}



public interface IPickable
{
    void PickItem();
}
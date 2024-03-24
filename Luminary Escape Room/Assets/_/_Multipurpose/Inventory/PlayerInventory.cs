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
    public string currentItemName;
    public string lastItemName;

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

    void WorldItemInteractions(string worldItemName)
    {
        if(currentItemName == "Key" && worldItemName == "Chest")
        {
            Debug.Log("item interaction");
        }
        if(currentItemName == "Fruit" && worldItemName == "Mortar")
        {
            Debug.Log("item interaction");
        }
    }

    void CombineItems()
    {
        if (currentItemName == "Key" && lastItemName == "Time" || currentItemName == "Time" && lastItemName == "Key")
        {
            Debug.Log("item combine");
        }
    }

    public void SlotClicked()
    {
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
            if (i < inventoryList.Count)
            {
                itemSprites[i].sprite = itemSetActive[inventoryList[i]].GetComponentInChildren<ItemPickable>().itemScriptableObject.item_sprite;
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
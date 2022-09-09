using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellPointVisualizer : MonoBehaviour
{
    [SerializeField] int iteratorJump = 1;
    [SerializeField] private GameObject pre_cell;
    [SerializeField] private GameObject parent;
    [SerializeField] private string filename;
    [SerializeField] List<Cell> cells = new List<Cell>();
    [SerializeField] private List<Color> colors;
    [SerializeField] private List<GameObject> objects = new List<GameObject>();
    private Dictionary<string, Color> colorKey = new Dictionary<string, Color>();
    private Dictionary<string, int> cellCounts = new Dictionary<string, int>();
    int counter = 0;

    [Header("User interface")]
    [SerializeField] Slider slider;
    [SerializeField] Button button;
    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_Text sliderValueText;
    [SerializeField] TMP_Text cellCountsText
        ;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(1);
        ReadCSV();
        colors = GetRandomColors();

        CreateCells(iteratorJump);
        SetUpUI();
    }

    void SetUpUI()
    {
        slider.value = iteratorJump;
        sliderValueText.text = Mathf.Ceil((cells.Count / slider.value)).ToString();
        slider.onValueChanged.AddListener(
            (value) => { sliderValueText.text = Mathf.Ceil((cells.Count / slider.value)).ToString(); }
            );
        button.onClick.AddListener(
            () =>
            {
                CreateCells((int)slider.value);
                statusText.text = ("Number of cells: " + counter).ToString();
            }
            );
    }

    void CreateCells(int jump)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }

        objects.Clear();

        counter = 0;
        for (int i = 0; i < cells.Count; i += jump)
        {
            GameObject cell = Instantiate(pre_cell, Vector3.zero, Quaternion.identity, parent.transform);
            cell.AddComponent<CellData>();
            CellData data = cell.GetComponent<CellData>();
            data.CellType = cells[i].CellType;
            cell.transform.position = new Vector3(-cells[i].Coords.x, cells[i].Coords.y, cells[i].Coords.z);
            Matrix4x4 matrixReflected = cell.transform.GetMatrix() * ReflectX();
            cell.transform.position = matrixReflected.GetPosition();
            cell.transform.rotation = matrixReflected.rotation;
            cell.transform.localScale = matrixReflected.lossyScale;
            //Material ;
            //cell.GetComponent<Renderer>().material;
            Color cellColor;
            colorKey.TryGetValue(cells[i].CellType, out cellColor);
            cell.transform.GetComponentInChildren<SpriteRenderer>().color = cellColor;
            counter++;
            objects.Add(cell);
        }

        foreach (var obj in objects)
        {
            if (!cellCounts.ContainsKey(obj.GetComponent<CellData>().CellType))
            {
                cellCounts.Add(obj.GetComponent<CellData>().CellType, 1);
            }
            else {
                cellCounts[obj.GetComponent<CellData>().CellType] += 1;
            };
        }

        cellCountsText.text = "";
        foreach (var item in cellCounts)
        {
            cellCountsText.text += item.Key + ": " + item.Value + "\n";

        }
       
    }

    List<Color> GetRandomColors()
    {
        List<Color> colors = new List<Color>();
        List<string> uniqueCellTypes = new List<string>();
        for (int i = 0; i < cells.Count; i++)
        {
            string toAdd = uniqueCellTypes.Contains(cells[i].CellType) ? null : cells[i].CellType;
            if (toAdd == null) continue;
            uniqueCellTypes.Add(toAdd);
            colors.Add(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), .6f));
            colorKey.Add(toAdd, colors[colors.Count - 1]);
        }
        return colors;
    }

    void ReadCSV()
    {
        using (var reader = new StreamReader("Assets/Data/" + filename + ".csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Split(",")[0] == "tissue_block_id") continue;
                cells.Add(new Cell(
                    line.Split(",")[0],
                    line.Split(",")[1],
                    line.Split(",")[2],
                    line.Split(",")[3],
                    new Coordinate(float.Parse(line.Split(",")[4]), float.Parse(line.Split(",")[5]), float.Parse(line.Split(",")[6]))
                    ));
            }
        }
    }

    Matrix4x4 ReflectX()
    {
        var result = new Matrix4x4(
            new Vector4(-1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, -1, 0),
            new Vector4(0, 0, 0, 1)
        );
        return result;
    }

    [Serializable]
    struct Cell
    {
        [SerializeField] private string tissueBlockId;
        [SerializeField] private string datasetId;
        [SerializeField] private string anatomicalStructure;
        public string CellType;
        public Coordinate Coords;

        public Cell(string tissueBlock, string dataset, string anatomicalStructure, string cellType, Coordinate coords)
        {
            this.tissueBlockId = tissueBlock;
            this.datasetId = dataset;
            this.anatomicalStructure = anatomicalStructure;
            this.CellType = cellType;
            this.Coords = coords;
        }
    }

    [Serializable]
    struct Coordinate
    {
        public float x, y, z;

        public Coordinate(float x, float y, float z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }
}

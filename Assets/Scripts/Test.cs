using UnityEngine;
using Table;

public class Test : MonoBehaviour
{
    void Start()
    {
        TableAccessor.LoadData();
        Debug.Log(TableAccessor.Student[TableAccessor.Student[Student.aaa].like].age);
    }
}
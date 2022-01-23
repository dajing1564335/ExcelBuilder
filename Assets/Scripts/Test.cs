using UnityEngine;
using Table;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public List<StudentData> Datas;

    void Start()
    {
        TableAccessor.LoadData();
        foreach (var student in TableAccessor.Student[Student.aaa].like)
        {
            Debug.Log(TableAccessor.Student[student].age);
        }
    }
}
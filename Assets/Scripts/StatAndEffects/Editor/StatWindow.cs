using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using StatAndEffects.Manager;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StatAndEffects.Editor
{
    public class StatWindow : EditorWindow
    {
        private TwoPaneSplitView _splitView;

        // LEFT MENU
        private ListView _menuList;
        private List<string> _menuItems = new()
        {
            "StatDefinitions", 
            "AttributesStatDefinitions",
            "Settings"
        };

        // RIGHT CONTENT
        private VisualElement _contentPanel;

        private List<StatDefinition> _statDefinitions;

        private List<AttributeStatDefinition> _attributes;
        [MenuItem("Tools/Stat And Effects")]
        public static void ShowWindow()
        {
            var window = GetWindow<StatWindow>();
            window.titleContent = new GUIContent("Stat And Effects");
            window.Show();
        }

        private void CreateGUI()
        {
            // 🔹 Split view
            _splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(_splitView);

            // =========================
            // LEFT MENU
            // =========================
            _menuList = new ListView
            {
                itemsSource = _menuItems,
                selectionType = SelectionType.Single,
                fixedItemHeight = 30
            };

            _menuList.makeItem = () => new Label();
            _menuList.bindItem = (e, i) =>
            {
                (e as Label).text = _menuItems[i];
            };

            _menuList.onSelectionChange += OnMenuChanged;

            _splitView.Add(_menuList);

            // =========================
            // RIGHT PANEL
            // =========================
            _contentPanel = new VisualElement();
            _contentPanel.style.flexGrow = 1;

            _splitView.Add(_contentPanel);

            LoadData();

            _menuList.SetSelection(0);
        }

        private void LoadData()
        {
            StatAbilitiesManager.LoadDatabase();
            _statDefinitions = StatAbilitiesManager.BaseStats.ToList();
            _attributes = _statDefinitions
                .OfType<AttributeStatDefinition>() // 🔥 filters + casts
                .ToList();
        }

        // =========================
        // 🔹 MENU SWITCH
        // =========================
        private void OnMenuChanged(IEnumerable<object> selected)
        {
            foreach (var item in selected)
            {
                string menu = item as string;

                _contentPanel.Clear();

                switch (menu)
                {
                    case "StatDefinitions":
                        DrawStatTable(); 
                        break;

                    case "AttributesStatDefinitions":
                        DrawAttributeTable();
                        break;
                    case "Settings":
                        DrawSettings();
                        break;
                }
            }
        }
        
        MultiColumnListView table;
        // =========================
        // 🔹 MULTI COLUMN TABLE
        // =========================
        private void DrawStatTable()
        {
            var toolbar = new VisualElement();
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.marginBottom = 6;

            var createStatBtn = new Button(() => CreateStat(false))
            {
                text = "Create Stat",
            };

            var createAttrBtn = new Button(() => CreateStat(true))
            {
                text = "Create Attribute Stat"
            };

            var SaveAssetbtn = new Button(() => SaveAssetsByName())
            {
                text = "Save Assets By Name",
            };
            
            var generateIdBtn = new Button(() => GenerateIds())
            {
                text = "Generate IDs"
            };

            toolbar.Add(generateIdBtn);

            toolbar.Add(createStatBtn);
            toolbar.Add(createAttrBtn);
            toolbar.Add(SaveAssetbtn);
            _contentPanel.Add(toolbar);
            
            table = new MultiColumnListView
            {
                itemsSource = this._statDefinitions,
                style = { flexGrow = 1 },
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                sortingMode = ColumnSortingMode.Default,
            };
            _contentPanel.Add(table);

            // =========================
            // COLUMNS
            // =========================

            table.columns.Add(new Column
            {
                title = "ID",
                minWidth = 60, // small column
                stretchable = true,
                makeCell = () =>
                {
                    var field = new IntegerField();
                    field.style.flexGrow = 1;
                    return field;
                },

                bindCell = (element, index) =>
                {
                    var field = element as IntegerField;
                    var stat = _statDefinitions[index];

                    // 🔹 set value
                    field.SetValueWithoutNotify(stat.Id);

                    // 🔹 avoid stacking callbacks (IMPORTANT)
                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Stat ID");

                        stat.Id = evt.newValue;

                        EditorUtility.SetDirty(stat);
                    });
                },
                comparison = (a, b) =>
                {
                    var stat = _statDefinitions[a];
                    var stat2 = _statDefinitions[b];
                    return stat.Id.CompareTo(stat2.Id);
                }
            });

            // 🔹 NAME
            table.columns.Add(new Column
            {
                title = "Display Name",
                stretchable = true,
                minWidth = 200,
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;

                    var textField = new TextField();
                    textField.style.flexGrow = 1;

                    var button = new Button();
                    button.text = "↑"; // small icon button
                    button.style.width = 24;
                    button.style.marginLeft = 4;

                    container.Add(textField);
                    container.Add(button);

                    return container;
                },

                bindCell = (element, index) =>
                {
                    var container = element as VisualElement;
                    
                    var textField = container.ElementAt(0) as TextField;
                    var button = container.ElementAt(1) as Button;

                    var stat = _statDefinitions[index];

                    // 🔹 Set initial value
                    textField.SetValueWithoutNotify(stat.DisplayName);

                    // 🔹 TEXT CHANGE
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Display Name");

                        var so = new SerializedObject(stat);
                        so.FindProperty("displayName").stringValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });

                    // 🔹 BUTTON CLICK → FORMAT
                    button.clicked -= null; // prevent stacking (optional safety)

                    button.clicked += () =>
                    {
                        string formatted = stat.DisplayName.FirstCharToUpper();

                        Undo.RecordObject(stat, "Format Display Name");

                        var so = new SerializedObject(stat);
                        so.FindProperty("displayName").stringValue = formatted;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);

                        textField.SetValueWithoutNotify(formatted);
                    };
                },
                comparison = (a, b) =>
                {
                    var sa = _statDefinitions[a];
                    var sb = _statDefinitions[b];
                    return string.
                        Compare(sa.DisplayName, sb.DisplayName,
                            StringComparison.OrdinalIgnoreCase);
                }
            });

            // 🔹 SHORT NAME
            table.columns.Add(new Column
            {
                title = "Short",
                stretchable = true,
                minWidth = 100,
                makeCell = () => new TextField(),
                bindCell = (element, index) =>
                {
                    var field = element as TextField;
                    var stat = _statDefinitions[index];

                    field.SetValueWithoutNotify(stat.ShortName);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Short Name");

                        var so = new SerializedObject(stat);
                        so.FindProperty("shortName").stringValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            // 🔹 DESCRIPTION
            table.columns.Add(new Column
            {
                title = "Description",
                stretchable = true,
                minWidth = 250,
                makeCell = () => new TextField() { multiline = true },
                bindCell = (element, index) =>
                {
                    var field = element as TextField;
                    var stat = _statDefinitions[index];

                    field.SetValueWithoutNotify(stat.Description);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Description");

                        var so = new SerializedObject(stat);
                        so.FindProperty("description").stringValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            // 🔹 ICON
            table.columns.Add(new Column
            {
                title = "Icon",
                stretchable = true,
                minWidth = 120,
                makeCell = () => new ObjectField { objectType = typeof(Sprite) },
                bindCell = (element, index) =>
                {
                    var field = element as ObjectField;
                    var stat = _statDefinitions[index];

                    field.SetValueWithoutNotify(stat.Icon);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Icon");

                        var so = new SerializedObject(stat);
                        so.FindProperty("icon").objectReferenceValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            _contentPanel.Add(table);
        }
        
        private void SaveAssetsByName()
        {
            foreach (StatDefinition statDefinition in this._statDefinitions)
            {
                if (statDefinition.DisplayName == null)
                    continue;
                var path = AssetDatabase.GetAssetPath(statDefinition);
                AssetDatabase.RenameAsset(path, statDefinition.DisplayName);
            }
        }

        private void DrawAttributeTable()
        {
            var toolbar = new VisualElement();
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.marginBottom = 6;

            var createAttrBtn = new Button(() => CreateStat(true))
            {
                text = "Create Attribute Stat"
            };


            toolbar.Add(createAttrBtn);

            _contentPanel.Add(toolbar);

            
            table = new MultiColumnListView
            {
                itemsSource = this._attributes,
                style = { flexGrow = 1 },
                selectionType = SelectionType.None,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };
            _contentPanel.Add(table);

            // =========================
            // COLUMNS
            // =========================

            table.columns.Add(new Column
            {
                title = "ID",
                minWidth = 60, // small column
                stretchable = true,
                makeCell = () =>
                {
                    var field = new IntegerField();
                    return field;
                },

                bindCell = (element, index) =>
                {
                    var field = element as IntegerField;
                    var stat = _attributes[index];

                    // 🔹 set value
                    field.SetValueWithoutNotify(stat.Id);

                    // 🔹 avoid stacking callbacks (IMPORTANT)
                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Stat ID");

                        stat.Id = evt.newValue;

                        EditorUtility.SetDirty(stat);
                    });
                }
            });
            // 🔹 NAME
            table.columns.Add(new Column
            {
                title = "Display Name",
                stretchable = true,
                minWidth = 200,
                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;

                    var textField = new TextField();
                    textField.style.flexGrow = 1;

                    var button = new Button();
                    button.text = "↑"; // small icon button
                    button.style.width = 24;
                    button.style.marginLeft = 4;

                    container.Add(textField);
                    container.Add(button);

                    return container;
                },

                bindCell = (element, index) =>
                {
                    var container = element as VisualElement;
                    
                    var textField = container.ElementAt(0) as TextField;
                    var button = container.ElementAt(1) as Button;

                    var stat = _attributes[index];

                    // 🔹 Set initial value
                    textField.SetValueWithoutNotify(stat.DisplayName);

                    // 🔹 TEXT CHANGE
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Display Name");

                        var so = new SerializedObject(stat);
                        so.FindProperty("displayName").stringValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });

                    // 🔹 BUTTON CLICK → FORMAT
                    button.clicked -= null; // prevent stacking (optional safety)

                    button.clicked += () =>
                    {
                        string formatted = stat.DisplayName.FirstCharToUpper();

                        Undo.RecordObject(stat, "Format Display Name");

                        var so = new SerializedObject(stat);
                        so.FindProperty("displayName").stringValue = formatted;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);

                        textField.SetValueWithoutNotify(formatted);
                    };
                }
            });

            // 🔹 SHORT NAME
            table.columns.Add(new Column
            {
                title = "Short",
                stretchable = true,
                minWidth = 100,
                makeCell = () => new TextField(),
                bindCell = (element, index) =>
                {
                    var field = element as TextField;
                    var stat = _attributes[index];

                    field.SetValueWithoutNotify(stat.ShortName);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Short Name");

                        var so = new SerializedObject(stat);
                        so.FindProperty("shortName").stringValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            // 🔹 DESCRIPTION
            table.columns.Add(new Column
            {
                title = "Description",
                stretchable = true,
                minWidth = 250,
                makeCell = () => new TextField() { multiline = true },
                bindCell = (element, index) =>
                {
                    var field = element as TextField;
                    var stat = _attributes[index];

                    field.SetValueWithoutNotify(stat.Description);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Description");

                        var so = new SerializedObject(stat);
                        so.FindProperty("description").stringValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            // 🔹 ICON
            table.columns.Add(new Column
            {
                title = "Icon",
                stretchable = true,
                minWidth = 120,
                makeCell = () => new ObjectField { objectType = typeof(Sprite) },
                bindCell = (element, index) =>
                {
                    var field = element as ObjectField;
                    var stat = _attributes[index];

                    field.SetValueWithoutNotify(stat.Icon);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Icon");

                        var so = new SerializedObject(stat);
                        so.FindProperty("icon").objectReferenceValue = evt.newValue;
                        so.ApplyModifiedProperties();

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            table.columns.Add(new Column
            {
                title = "Attributes",
                stretchable = true,
                minWidth = 400,

                makeCell = () =>
                {
                    var container = new VisualElement();
                    container.style.flexGrow = 1;
                    return container;
                },

                bindCell = (element, index) =>
                {
                    var container = element as VisualElement;
                    container.Clear();

                    var stat = _attributes[index];

                    DrawAttributeDataTable(container, stat);
                }
            });
            
            _contentPanel.Add(table);
        }

        private void DrawAttributeDataTable(VisualElement root, AttributeStatDefinition stat)
        {
            var listView = new MultiColumnListView
            {
                itemsSource = stat.attributesDatas,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            // =========================
            // TARGET STAT
            // =========================
            listView.columns.Add(new Column
            {
                title = "Stat",
                width = 120,
                makeCell = () => new ObjectField { objectType = typeof(StatDefinition) },

                bindCell = (element, i) =>
                {
                    var field = element as ObjectField;
                    var data = stat.attributesDatas[i];

                    field.SetValueWithoutNotify(data.statDefinition);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Change Target Stat");

                        data.statDefinition = evt.newValue as StatDefinition;

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            // =========================
            // MODIFIER TYPE
            // =========================
            listView.columns.Add(new Column
            {
                title = "Type",
                width = 100,
                makeCell = () => new EnumField(StatModifierType.Flat),

                bindCell = (element, i) =>
                {
                    var field = element as EnumField;
                    var data = stat.attributesDatas[i];

                    field.SetValueWithoutNotify(data.statModifierType);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Change Modifier Type");

                        data.statModifierType = (StatModifierType)evt.newValue;

                        EditorUtility.SetDirty(stat);
                    });
                }
            });

            // =========================
            // FORMULA
            // =========================
            listView.columns.Add(new Column
            {
                title = "Formula",
                stretchable = true,
                makeCell = () => new TextField(),
    
                bindCell = (element, i) =>
                {
                    var field = element as TextField;
                    var data = stat.attributesDatas[i];

                    field.SetValueWithoutNotify(data.Formula);

                    field.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(stat, "Edit Formula");

                        data.Formula = evt.newValue;
                        EditorUtility.SetDirty(stat);
                    });
                }
            });


            root.Add(listView);

            // ➕ ADD BUTTON
            var addBtn = new Button(() =>
            {
                Undo.RecordObject(stat, "Add Attribute");

                stat.attributesDatas.Add(new AttributeData());

                EditorUtility.SetDirty(stat);
                listView.RefreshItems();
            })
            {
                text = "+"
            };
            // ➖ REMOVE SELECTED
            var removeBtn = new Button(() =>
            {
                int index = listView.selectedIndex;

                if (index < 0 || index >= stat.attributesDatas.Count)
                    return;

                Undo.RecordObject(stat, "Remove Attribute");

                stat.attributesDatas.RemoveAt(index);

                EditorUtility.SetDirty(stat);
                listView.RefreshItems();
            })
            {
                text = "-"
            };

            root.Add(addBtn);
            root.Add(removeBtn);
        }
        // =========================
        // 🔹 SETTINGS
        // =========================
        private void DrawSettings()
        {
            _contentPanel.Add(new Label("Settings Panel"));
        }
        private void GenerateIds()
        {
            Undo.RecordObjects(_statDefinitions.ToArray(), "Generate Stat IDs");

            // 1️⃣ Split into groups
            var normalStats = new List<StatDefinition>();
            var attributeStats = new List<StatDefinition>();

            foreach (var stat in _statDefinitions)
            {
                if (stat is AttributeStatDefinition)
                    attributeStats.Add(stat);
                else
                    normalStats.Add(stat);
            }

            // 2️⃣ Sort each group by DisplayName
            normalStats.Sort((a, b) =>
                string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            attributeStats.Sort((a, b) =>
                string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            // 3️⃣ Assign IDs
            int id = 1;

            foreach (var stat in normalStats)
            {
                stat.Id = id++;
                EditorUtility.SetDirty(stat);
            }

            foreach (var stat in attributeStats)
            {
                stat.Id = id++;
                EditorUtility.SetDirty(stat);
            }

            AssetDatabase.SaveAssets();
            this.table.RefreshItems();
        }
        private void CreateStat(bool isAttribute)
        {
#if UNITY_EDITOR
            string path = isAttribute ? StatAbilitiesManager.Database.AttributeStatDefinitionPath : StatAbilitiesManager.Database.StatDefinitionPath; 

            string name = isAttribute ? "NewAttributeStat" : "NewStat";

            string assetPath = EditorUtility.SaveFilePanelInProject(
                "Create Stat",
                name,
                "asset",
                "Select location",
                path
            );

            if (string.IsNullOrEmpty(assetPath))
                return;

            ScriptableObject asset;

            if (isAttribute)
                asset = ScriptableObject.CreateInstance<AttributeStatDefinition>();
            else
                asset = ScriptableObject.CreateInstance<StatDefinition>();

            StatAbilitiesManager.AddStat(asset as StatDefinition);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(asset);
            
            RefreshDatabaseAndUI(); // 👈 important
#endif
        }
        
        private void RefreshDatabaseAndUI()
        {
#if UNITY_EDITOR

            // reload local list
            _statDefinitions = StatAbilitiesManager.BaseStats.ToList();

            // redraw UI
            _contentPanel.Clear();
            DrawStatTable();
#endif
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

using Newtonsoft.Json;                          // JSON Package
using Proyecto26;                               // RestClient package (wrapper over UnityWebRequests)

using set;                                      // Set type matching the JSON representation of a Set
using card;
using Editor.Events;
using Editor.Listeners;


namespace Editor
{
    public class PokeTCG : EditorWindow
    {
       #region Events Listeners and Windows
        private static CustomLogWindow _debugLog;
        private RequestMadeEventListener _requestMadeEventListener;
        private RequestMadeEvent _requestMadeEvent;
        #endregion Events Listeners and Windows
       #region Header Text Fields
        private Rect hostRect = new Rect(5, 5, 260, 20);
        private Rect queryTextRect = new Rect(10, 40, 300, 300);
        private Rect filterRect = new Rect(320, 5, 350, 200);
        private Rect buttonRowRect = new Rect(0, 150, 800, 100);
        private Rect lowerOutputPanel = new Rect(5, 200, 790, 550);
        private Rect buttonColumnRect = new Rect(680, 5, 110, 200);
        
        private static string _host = "https://api.pokemontcg.io/v2/";
        private static string _query = "";
        private static string _page = "1";
        private static string _pageSize = "10";
        private static string _orderBy = "";
        #endregion
       #region  GUILayoutOption Definitions
        private GUILayoutOption[] noExpandOption = {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)};
        private GUILayoutOption[] expandWidthOption = {GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)};
        private GUILayoutOption[] expandHeightOption = {GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)};
        private GUILayoutOption[] expandWidthHeightOption = {GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)};
        #endregion
       #region Web Request Fields
        private RequestHelper _requestOptions;
        private ResponseHelper _myResponse;
        private MultiSetRequest _multiSetRequest;
        private MultiCardRequest _multiCardRequest;
        
        private const string APIKeyValue = "b9afbddc-7eff-4954-82dd-d4ab470e94f7";
        private const string APIKeyName = "X-Api-Key";
        #endregion
       #region Search Types, Indeces, Editor Window Fields
        private static readonly string[] SearchType = {"Cards", "Sets"};
        private static int _sgSelected = 0;
        private static Vector2 _scrollPosition;
        private static int _setIndex = 0;
        private static int _cardIdx = 0;
        private static PokeTCG _window;
        private enum ImageType
        {
            Logo,
            Symbol,
            Front,
            Back
        };
        #endregion
       #region Texture Fields and Dictionary
        private static Texture _cardFrontTexture;
        private static Texture _setSymbolTexture;
        private static Texture _setLogoTexture;
        private static Dictionary<string, Texture> _textureDict;
        #endregion
       #region Card Info Display Filters
        bool _idFilter = true;
        bool _nameFilter = true;
        bool _superTypeFilter = true;
        bool _subTypeFilter = true;
        bool _levelFilter = true;
        bool _hpFilter = true;
        bool _typeFilter = true;
        bool _fEvoFilter = true;
        bool _tEvoFilter = true;
        bool _ruleFilter = true;
        bool _ancTFilter = true;
        bool _ablFilter = true;
        bool _atkFilter = true;
        bool _wknsFilter = true;
        bool _resistFilter = true;
        bool _rtrCostFilter = true;
        bool _convRtrCostFilter = true;
        bool _setFilter = true;
        bool _numFilter = true;
        bool _artistFilter = true;
        bool _rarityFilter = true;
        bool _textFilter = true;
        bool _imageFilter = true;

        

        #endregion
       #region Constructors
       public PokeTCG()
       {
           _requestMadeEventListener = new RequestMadeEventListener();
           _requestMadeEvent = new RequestMadeEvent();
           _requestMadeEvent.RegisterListener(_requestMadeEventListener);
           _debugLog = new CustomLogWindow(ref _requestMadeEventListener);
       }
       #endregion Constructors
       #region Static Methods
       [MenuItem("Window/pokeGET/Launch")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            _window = (PokeTCG) EditorWindow.GetWindow(typeof(PokeTCG));
            _window.titleContent = new GUIContent("Pokemon-TCG REST-API");
            _window.minSize = new Vector2(800, 750);
            _window.maxSize = new Vector2(800, 750);
            _window.maximized = true;
            _window.Show();
        }
        
      
        [MenuItem("Window/pokeGET/Force Stop")]
        public static void ForceStop()
        {
            if(_window != null) _window.Close();
        }
        #endregion Static Methods
       #region Overrides
        private void OnDestroy()
        {
            Debug.Log("Window.OnDestroy()");
            CustomLogWindow.CloseConsole();
        }

        private void OnGUI()
        {
           PrintHeaderPanel();
           PrintButtonRowPanel();
           PrintLowerOutputPanel();
        }
#endregion
       #region Printing Functions
        private void PrintLowerOutputPanel()
        {
            
            GUILayout.BeginArea(lowerOutputPanel);
            EditorGUILayout.BeginHorizontal(/*EditorStyles.helpBox*/);
            PrintLeftOutputPanel();
            PrintRightOutputPanel();
            EditorGUILayout.EndHorizontal(); // This is the horizontal frame containing the Outputpanels
            GUILayout.EndArea();
        }

        private void PrintRightOutputPanel()
        {
              #region Right-Side Vertical Image Output Panel
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
           
                switch (SearchType[_sgSelected])
                {
                   case "Cards":
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(string.Format("<<"), new GUILayoutOption[]{GUILayout.ExpandWidth(true)}))  // First Card
                            {
                                if (_multiCardRequest == null || _multiCardRequest.data == null) return;
                                _cardIdx = 0;
                               
                               
                                RequestImage(_multiCardRequest.data[_cardIdx].images.large, ImageType.Front);
                                _window.Repaint();
                            }

                          
                            if (GUILayout.Button(string.Format("<-"), new GUILayoutOption[]{GUILayout.ExpandWidth(true)})) // Previous Card
                            {
                                if (((_cardIdx - 1) < 0) || (_multiCardRequest == null) || _multiCardRequest.data == null) return;

                                --_cardIdx;
                                //_text = _multiCardRequest.data[_cardIdx].ToString();
                               RequestImage(_multiCardRequest.data[_cardIdx].images.large, ImageType.Front);
                               //_window.Repaint();

                            }

                            if (GUILayout.Button(string.Format("->"), new GUILayoutOption[]{GUILayout.ExpandWidth(true)}))  // Next Card
                            {
                                if (((_cardIdx + 1) >= _multiCardRequest.data.Length) || (_multiCardRequest == null) || _multiCardRequest.data == null) return;

                                ++_cardIdx;
                                //_text = _multiCardRequest.data[_cardIdx].ToString();
                                
                                RequestImage(_multiCardRequest.data[_cardIdx].images.large, ImageType.Front);
                            }
                            
                            if (GUILayout.Button(">>", new GUILayoutOption[]{GUILayout.ExpandWidth(true)})) // Last Card
                            {
                                if (_multiCardRequest == null || _multiCardRequest.data == null) return;
                                _cardIdx = _multiCardRequest.data.Length-1;
                                //_text = _multiCardRequest.data[_cardIdx].ToString();
                                RequestImage(_multiCardRequest.data[_cardIdx].images.large, ImageType.Front);
                                
                            }
                            
                            
                            
                        GUILayout.EndHorizontal();
                    
                        GUILayout.BeginVertical();
                                GUILayoutOption[] frontLayout =
                                {
                                    GUILayout.Width(500*0.75f),
                                    GUILayout.Height(700*0.75f)
                                    
                                };
                                GUILayout.Label(_cardFrontTexture, frontLayout);
                        GUILayout.EndVertical();
                        break;
                    }
                   
                    case "Sets":
                    {
                        GUILayout.BeginHorizontal();
                                if (GUILayout.Button("<-"))
                                {
                                    if (((_setIndex - 1) < 0) || (_multiSetRequest == null) ||(_multiSetRequest.data)== null) return;

                                    --_setIndex;
                                  //  _text = _multiSetRequest.data[_setIndex].ToString();
                                    RequestImage(_multiSetRequest.data[_setIndex].images.symbol, ImageType.Symbol);
                                    RequestImage(_multiSetRequest.data[_setIndex].images.logo, ImageType.Logo);
                                }

                                if (GUILayout.Button("->"))
                                {
                                    if (((_setIndex + 1) >= _multiSetRequest.data.Length) || (_multiSetRequest == null)) return;

                                    ++_setIndex;
                                //    _text = _multiSetRequest.data[_setIndex].ToString();
                                    RequestImage(_multiSetRequest.data[_setIndex].images.symbol, ImageType.Symbol);
                                    RequestImage(_multiSetRequest.data[_setIndex].images.logo, ImageType.Logo);
                                }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginVertical();
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                                    GUILayoutOption[] symbolLayout = {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),  GUILayout.Width(250.0f), GUILayout.Height(250.0f)};
                                    GUILayout.Label(_setSymbolTexture, symbolLayout);
                                    
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                                GUILayoutOption[] logoLayout = {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),  GUILayout.Width(250.0f), GUILayout.Height(250.0f)};
                                GUILayout.Label(_setLogoTexture, logoLayout);
                            GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        
                        break;
                    }
                }
                
                EditorGUILayout.EndVertical();
                #endregion // End Right-Side Vertical Image Output Panel
        }

        private void PrintFormattedCardData(CardData cardData)
        {
            if (cardData == null) return;
            GUIStyle boldStyle = new GUIStyle();
            boldStyle.richText = true;
            
            if(_idFilter) EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>ID:</b></color> <color=WHITE>{0}</color></size>", cardData.id), boldStyle);
            if(_nameFilter) EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>Name:</b></color> <color=WHITE>{0}</color></size>", cardData.name), boldStyle);
            if(_superTypeFilter) EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>Supertype:</b></color> <color=WHITE>{0}</color></size>", cardData.supertype), boldStyle);
            if (_subTypeFilter)
            {
                EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>Subtypes:</b></color></size>"), boldStyle);

                foreach (var sT in cardData.subtypes) 
                {
                    EditorGUILayout.LabelField(string.Format("<size=15><color=WHITE>\t{0}</color></size>",sT), boldStyle);
                }
            }
            
            if(_levelFilter) EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>Level:</b></color> <color=WHITE>{0}</color></size>", cardData.level), boldStyle);
            
            if(_hpFilter) EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>HP:</b></color> <color=WHITE>{0}</color></size>", cardData.hp), boldStyle);
             
            
            if (_typeFilter)
            {
                EditorGUILayout.LabelField(string.Format("<size=15><color=YELLOW><b>Types:</b></color></size>"), boldStyle);

                foreach (var type in cardData.types) 
                {
                    EditorGUILayout.LabelField(string.Format("<size=15><color=WHITE>\t{0}</color></size>",type), boldStyle);
                }
            }
            
        }

        private void PrintLeftOutputPanel()
        {
            #region Left Output Panel
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _scrollPosition = EditorGUILayout.BeginScrollView(
                _scrollPosition,
                alwaysShowHorizontal: false,
                alwaysShowVertical: false,
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(false),
                GUILayout.Width(position.width*.50f)
            );
                 
            PrintFormattedCardData(_multiCardRequest.data[_cardIdx]);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
           #endregion // Left output panel
        }

        private void PrintButtonRowPanel()
        {
            
            GUILayout.BeginArea(buttonRowRect);
            GUILayoutOption[] selGridOpts = expandWidthOption;
            
            GUILayout.BeginHorizontal(EditorStyles.helpBox, selGridOpts);
            _sgSelected = GUILayout.SelectionGrid(_sgSelected, SearchType, SearchType.Length, selGridOpts);
            GUILayout.Space(5);
            
            
            if (GUILayout.Button("Submit", expandWidthOption))
            {
                //_debugLog.Log("Submit Button Pressed");
                Get();
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void PrintHeaderPanel()
        {
            GUILayout.Space(10);
             GUILayout.BeginHorizontal();

             PrintHostLabel();
             PrintQueryTextFields();
             
             GUILayout.BeginHorizontal(/*EditorStyles.helpBox*/);
                    if (SearchType[_sgSelected] == "Cards" )
                    {
                        PrintFilterToggles();

                        GUILayout.BeginArea(buttonColumnRect);
                        GUILayout.BeginVertical(); // 4 Button Column top right of screen
                            GUILayout.Space(2.75f);
                            if(GUILayout.Button("Generate SOs", expandWidthOption))
                            {
                                if (_multiCardRequest != null)
                                {
                                    ExportCardsAsSO(_multiCardRequest);
                                }
                            }
                            GUILayout.Space(2.75f);
                        
                            if(GUILayout.Button("Debug Log", expandWidthOption))
                            {
                               
                                //_debugLog = PokeTCG_Debug_Output.Init();
                                CustomLogWindow.OpenWindow();
                            }
                            GUILayout.Space(2.75f);
                        
                        if(GUILayout.Button("Close Log", expandWidthOption))
                        {
                            CustomLogWindow.CloseConsole();
                        }
                        
                        GUILayout.Space(2.75f);
                        
                        if(GUILayout.Button("Clear Cache", expandWidthOption))
                        {
                            _textureDict.Clear();
                        }
                        
                        GUILayout.Space(2.75f);
                        
                        if(GUILayout.Button("About", expandWidthOption))
                        {
                            EditorUtility.DisplayDialog(title: "About",
                                message: "This is a button to explain this program.", "Rad");
                        }
                        
                        GUILayout.Space(2.75f);
                        
                        GUILayout.EndVertical();
                        GUILayout.EndArea();
                    }
                    GUILayout.EndHorizontal();

                    
            GUILayout.EndHorizontal();
        }

        private void PrintHostLabel()
        {
            GUILayout.BeginArea(hostRect);
           
            
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.richText = true;
            
            EditorGUILayout.SelectableLabel(  string.Format("<color=WHITE><size=15><b> Host:   </b></size></color><color=MAGENTA><size=15>{0}</size></color>",_host), guiStyle);
            GUILayout.EndArea();
        }

        private void PrintFilterToggles()
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.richText = true;
            
            GUILayout.BeginArea(filterRect);
            GUILayout.BeginVertical(EditorStyles.helpBox); // Area between text fields and buttons for filters
                            GUILayout.Label( string.Format("<color=WHITE><b>Filters:</b></color>"), guiStyle);
                            var oldWidth = EditorGUIUtility.labelWidth;
                            GUILayout.BeginHorizontal();
                            // TODO this next section (and all of OnGUI() for that matter) need to be refactored, this is prototype level coding !
                                EditorGUIUtility.labelWidth = 1.0f;
                                _idFilter = EditorGUILayout.ToggleLeft("ID#", _idFilter/*, GUILayout.Width(50)*/);
                                _nameFilter = EditorGUILayout.ToggleLeft("Name", _nameFilter/*, GUILayout.Width(50)*/);
                                _superTypeFilter = EditorGUILayout.ToggleLeft("SupT", _superTypeFilter/*, GUILayout.Width(50)*/);
                                _subTypeFilter = EditorGUILayout.ToggleLeft("SubT", _subTypeFilter/*, GUILayout.Width(50)*/);
                                _levelFilter = EditorGUILayout.ToggleLeft("Lvl", _levelFilter/*, GUILayout.Width(50)*/);
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal();
                            {
                                _hpFilter = EditorGUILayout.ToggleLeft("HP", _hpFilter);
                                _typeFilter = EditorGUILayout.ToggleLeft("Type", _typeFilter);
                                _fEvoFilter = EditorGUILayout.ToggleLeft("EvoF", _fEvoFilter);
                                _tEvoFilter = EditorGUILayout.ToggleLeft("EvoT", _tEvoFilter);
                                _ruleFilter = EditorGUILayout.ToggleLeft("Rule", _ruleFilter);
                            }
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal();
                            {
                                _ancTFilter = EditorGUILayout.ToggleLeft("AncT", _ancTFilter);
                                _ablFilter = EditorGUILayout.ToggleLeft("Abl", _ablFilter);
                                _atkFilter = EditorGUILayout.ToggleLeft("Atk", _atkFilter);
                                _wknsFilter = EditorGUILayout.ToggleLeft("Wkns", _wknsFilter);
                                _resistFilter = EditorGUILayout.ToggleLeft("Rst", _resistFilter);
                            }
                            GUILayout.EndHorizontal();
                                
                            GUILayout.BeginHorizontal();
                            {
                                _rtrCostFilter = EditorGUILayout.ToggleLeft("RtC", _rtrCostFilter);
                                _convRtrCostFilter = EditorGUILayout.ToggleLeft("cRtC", _convRtrCostFilter);
                                _setFilter = EditorGUILayout.ToggleLeft("Set", _setFilter);
                                _numFilter = EditorGUILayout.ToggleLeft("Num", _numFilter);
                                _artistFilter = EditorGUILayout.ToggleLeft("Arts", _artistFilter);
                            }
                            GUILayout.EndHorizontal();
                            
                            GUILayout.BeginHorizontal();
                            {
                                _rarityFilter = EditorGUILayout.ToggleLeft("RarT", _rarityFilter);
                                _textFilter = EditorGUILayout.ToggleLeft("Txt", _textFilter);
                                _imageFilter = EditorGUILayout.ToggleLeft("Img", _imageFilter);
                                

                                if (GUILayout.Button("Select All"))
                                {
                                    _idFilter = _nameFilter = _superTypeFilter = _subTypeFilter =
                                        _levelFilter = _hpFilter = _typeFilter = _fEvoFilter =
                                            _tEvoFilter = _ruleFilter = _ancTFilter =
                                                _ablFilter = _atkFilter = _wknsFilter =
                                                    _resistFilter = _rtrCostFilter = _convRtrCostFilter =
                                                        _setFilter = _numFilter = _artistFilter =
                                                            _rarityFilter = _textFilter =
                                                                _imageFilter =  true;
                                }

                                if (GUILayout.Button("None"))
                                {
                                    _idFilter = _nameFilter = _superTypeFilter = _subTypeFilter =
                                        _levelFilter = _hpFilter = _typeFilter = _fEvoFilter =
                                            _tEvoFilter = _ruleFilter = _ancTFilter =
                                                _ablFilter = _atkFilter = _wknsFilter =
                                                    _resistFilter = _rtrCostFilter = _convRtrCostFilter =
                                                        _setFilter = _numFilter = _artistFilter =
                                                            _rarityFilter = _textFilter =
                                                                _imageFilter =  false;
                                }
                            }
                            GUILayout.EndHorizontal();
                            EditorGUIUtility.labelWidth = oldWidth;
                        GUILayout.EndVertical();
                        GUILayout.EndArea();
                        
        }

        private void PrintQueryTextFields()
        {
            GUILayout.BeginArea(queryTextRect);
            var orgWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 70;
            GUILayout.BeginVertical();
                _query = EditorGUILayout.TextField("q=", _query, GUILayout.Width(position.width * .400f));
                GUILayout.Space(3);
                
                _page = EditorGUILayout.TextField("page=", _page, GUILayout.Width(position.width * .400f));
                GUILayout.Space(3);
                
                _pageSize = EditorGUILayout.TextField("pageSize=", _pageSize,
                    GUILayout.Width(position.width * .400f));
                GUILayout.Space(3);
                
                _orderBy = EditorGUILayout.TextField("orderBy=", _orderBy,
                    GUILayout.Width(position.width * .400f));
                //GUILayout.Space(5);
                
            EditorGUIUtility.labelWidth = orgWidth;
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        #endregion Printing Functions
       #region Request Methods
        private void Get()
        {
            var searchTypeString = SearchType[_sgSelected].ToLower(); // initialize searchTypeString for url concatenation
           
                _requestOptions = new RequestHelper
                {
                    Uri = _host + searchTypeString,
                    Method = "GET",
                    Timeout = 10,
                    Params = new Dictionary<string, string>
                    {
                        {"q", _query},
                        {"page", _page},
                        {"pageSize", _pageSize},
                        {"orderBy", _orderBy}
                    },
                    Headers = new Dictionary<string, string> {{APIKeyName, APIKeyValue}},
                    ContentType = "application/json", //JSON is used by default
                    Retries = 3, //Number of retries
                    RetrySecondsDelay = 2, //Seconds of delay to make a retry
                    RetryCallback = (err, retries) => { }, //See the error before retrying the request
                    EnableDebug = true, //See logs of the requests for debug mode
                    IgnoreHttpException = true, //Prevent to catch http exceptions
                    UseHttpContinue = true,
                    RedirectLimit = 32
                };

            EditorCoroutineUtility.StartCoroutineOwnerless(HttpBase.DefaultUnityWebRequest(_requestOptions, (err, response) =>
                {
                    if (err != null)
                    {
                        EditorUtility.DisplayDialog("Error Response", err.Response, "Ok");
                    }
                    else // There was no problem == request successful!
                    {
                        _myResponse = response;
                        DeserializeBySearchType(SearchType[_sgSelected], _myResponse);
                    }
                }));
        }

        private void DeserializeBySearchType(string v, ResponseHelper res)
        {
            switch (v)
            {
                case "Sets": 
                {
                    _setIndex = 0;
                    _multiSetRequest = new MultiSetRequest();
                    _multiSetRequest = JsonConvert.DeserializeObject<set.MultiSetRequest>(res.Request.downloadHandler.text);
              //      _text += "\n" + _multiSetRequest.data[0].ToString();
                   RequestImage(_multiSetRequest.data[0].images.symbol, ImageType.Symbol);
                   RequestImage(_multiSetRequest.data[0].images.logo, ImageType.Logo);
                    GetWindow<PokeTCG>().Repaint();
                    break;
                }
                case "Cards": 
                {
                    _cardIdx = 0;
                    _multiCardRequest = new MultiCardRequest();
                    _multiCardRequest = JsonConvert.DeserializeObject<card.MultiCardRequest>(res.Request.downloadHandler.text);
            //        _text += "\n" + _multiCardRequest.data[0].ToString();
                    RequestImage(_multiCardRequest.data[0].images.large, ImageType.Front);
                    GetWindow<PokeTCG>().Repaint();
                    break;
                }
            }
        }
        
        private void RequestImage(string url, ImageType imgT)
        {
           
            _requestMadeEvent.Raise();
            Debug.Log("Image Requested: " + url);
            
            if (_textureDict == null) _textureDict = new Dictionary<string, Texture>();
            if (!_textureDict.ContainsKey(url))
            {
                var req = UnityWebRequestTexture.GetTexture(url);
                EditorCoroutineUtility.StartCoroutine(DownloadImage(url, imgT, req), this);
            }
            else
            {
                SetImageByType(url, imgT);
            }
            
        }

        void SetImageByType(string url, ImageType imgT)
        {
            switch (imgT)
            {
                case ImageType.Logo:
                    _setLogoTexture = _textureDict[url];
                    break;
                case ImageType.Symbol:
                    _setSymbolTexture = _textureDict[url];
                    break;
                case ImageType.Front:
                    _cardFrontTexture = _textureDict[url];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imgT), imgT, null);
            }
        }
        private IEnumerator DownloadImage(string url, ImageType imgT, UnityWebRequest wReq)
        {
           
            yield return wReq.SendWebRequest();
            if (wReq.responseCode != 200) yield break;
            if (!wReq.isDone) yield break;
            
            var tex = new Texture2D(0, 0);
            tex = ((DownloadHandlerTexture) wReq.downloadHandler).texture;
            
            _textureDict.Add(url, tex);
            
            Debug.Log("New Texture Dict entry: " + url);
            
            if (!_textureDict.TryGetValue(url, out Texture tempTexture2)) yield break;
            SetImageByType(url, imgT);





        }
        #endregion Request Methods
       #region Utility Methods (For features like exporting card data)
        private void ExportCardsAsSO(MultiCardRequest mcReq)
        {
            int cardNum = 1;
            string path = "";// = string.Format("Assets/DeckData/HGSS1/card{0}", cardNum);
            List<LineItem> lItems = new List<LineItem>();
            foreach (var card in mcReq.data)
            {
                path = string.Format("Assets/DeckData/Decks/MindFlood/Roster/{0}.asset", card.name);
                
                lItems.Add(ScriptableObject.CreateInstance<LineItem>().Init(card.set.id, 0, card.id));
           
                AssetDatabase.CreateAsset(lItems[cardNum-1], path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = lItems[cardNum-1];
                ++cardNum;
            }
        }
        #endregion Utility Methods (For features like exporting card data)
    }
}




using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSFramework.Application.Entity.SystemManage.ViewModel;

namespace BSFramework.Application.Web.Areas.SystemManage.Models
{
    /// <summary>
    /// 菜单树
    /// </summary>
    public class MenuTreeModel
    {
        private List<MenuTreeModel> _childNodes;
        private int _checkstate;
        private bool _complete;
        private bool _hasChildren;
        private string _id;
        private bool _isexpand;
        private string _parentnodes;
        private bool _showcheck;
        private string _value;

        public MenuTreeModel()
        {
            _checkstate = 0;
            _complete = true;
            _hasChildren = false;
            _id = "0";
            _isexpand = false;
            _parentnodes = "0";
            _showcheck = false;
            _value = "0";
            _childNodes = new List<MenuTreeModel>();
        }
        public MenuTreeModel(MenuConfigEntity item, string parentId = "0", bool showCheckBox = false,int checkstate=0) : this()
        {
            this.id = item.Id;
            this.text = item.ModuleName;
            this.value = item.Id;
            this.parentnodes = parentId;
            this.platformType = item.PaltformType;
            this.showcheck = showCheckBox;
            this.checkstate = checkstate;
        }

        public List<MenuTreeModel> ChildNodes
        {
            get { return _childNodes; }
            set
            {
                _childNodes = value;
            }
        }
        public int checkstate { get { return _checkstate; } set { _checkstate = value; } }
        public bool complete { get { return _complete; } set { _complete = value; } }
        public bool hasChildren { get { return _hasChildren; } set { _hasChildren = value; } } 
        public string id { get { return _id; } set { _id = value; } } 
        public string img { get; set; }
        public bool isexpand { get { return _isexpand; } set { _isexpand = value; } }
        public string parentnodes { get { return _parentnodes; } set { _parentnodes = value; } }
        public bool showcheck { get { return _showcheck; } set { _showcheck = value; } }
        public string text { get; set; }
        public string value { get { return _value; } set { _value = value; } }
        public int? platformType { get; set; }
        public void AddChild(MenuTreeModel menuTreeModel)
        {
            this.hasChildren = true;
            this.isexpand = true;
            this.ChildNodes.Add(menuTreeModel);
        }
    }

    public static class MenuTreeHelper
    {
        /// <summary>
        /// 生产  三个平台的集合
        /// </summary>
        /// <returns></returns>
        public static List<MenuTreeModel> InitData()
        {
            List<MenuTreeModel> list = new List<MenuTreeModel>();
            MenuTreeModel tree = new MenuTreeModel();
            tree.text = "安卓终端";
            tree.id = "1";
            tree.value = "1";
            tree.platformType = 1;
            list.Add(tree);
            MenuTreeModel tree1 = new MenuTreeModel();
            tree1.text = "Windows终端";
            tree1.id = "0";
            tree1.value = "0";
            tree1.platformType = 0;
            list.Add(tree1);
            MenuTreeModel tree2 = new MenuTreeModel();
            tree2.text = "手机APP";
            tree2.id = "2";
            tree2.value = "2";
            tree2.platformType = 2;
            list.Add(tree2);
            return list;
        }


        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="treeModel">包含三个平台树的数据</param>
        /// <param name="item">下级菜单</param>
        /// <param name="showCheckBox">是否显示单选钮</param>
        public static void FomateTree(List<MenuTreeModel> treeModel, MenuConfigEntity item, bool showCheckBox = false)
        {
            foreach (var x in treeModel)
            {
                bool matched = false;
                switch (item.PaltformType)
                {
                    case 0:
                        if (x.text == "Windows终端")
                        {
                            x.AddChild(new MenuTreeModel(item, x.id, showCheckBox));
                            matched = true;
                        }
                        break;
                    case 1:
                        if (x.text == "安卓终端")
                        {
                            x.AddChild(new MenuTreeModel(item, x.id, showCheckBox));
                            matched = true;
                        }
                        break;
                    case 2:
                        if (x.text == "手机APP")
                        {
                            x.AddChild(new MenuTreeModel(item, x.id, showCheckBox));
                            matched = true;
                        }
                        break;
                    default:
                        break;
                }
                if (matched) break;
            }

        }

        /// <summary>
        /// 生产无限级菜单
        /// </summary>
        /// <param name="firstLevel"></param>
        /// <param name="data"></param>
        /// <param name="showCheckBox">是否显示复选框</param>
        /// <param name="firstlevelShowCheckBox">第一级的复选框是否显示</param>
        public static void FomateTree(MenuTreeModel firstLevel, List<MenuConfigEntity> data, bool showCheckBox=false,bool firstlevelShowCheckBox=false)
        {
            firstLevel.showcheck = firstlevelShowCheckBox;
            var childList = data.Where(x => x.ParentId == firstLevel.id).ToList();
            childList.ForEach(p =>
            {
                firstLevel.AddChild(new MenuTreeModel(p, firstLevel.id, showCheckBox));
                    //加入到一级树下面
            });
            firstLevel.ChildNodes.ForEach(x =>
            {
                x.showcheck = showCheckBox;
                BuildTree(x, data, showCheckBox);
            });
        }

        /// <summary>
        /// 生产无限级菜单 ,根据传入的授权菜单的ID，来判断数据是否需要被勾上 （checkbox）
        /// </summary>
        /// <param name="firstLevel"></param>
        /// <param name="data"></param>
        /// <param name="AuthIds">已经授权过的菜单的ID，用来判断页面上的checkbox是不是应该勾上</param>
        public static void FomateTree(MenuTreeModel firstLevel, List<MenuConfigEntity> data,List<string> AuthIds, bool firstlevelShowCheckBox = false)
        {
            var childList = data.Where(x => x.ParentId == firstLevel.id).ToList();
            firstLevel.showcheck = firstlevelShowCheckBox;
            bool firstHas = AuthIds.Any(x => x == firstLevel.id);
            firstLevel.checkstate = firstHas ? 1 : 0;
            childList.ForEach(p =>
            {
                bool has = AuthIds.Any(x => x == p.ModuleId);
                firstLevel.AddChild(new MenuTreeModel(p, firstLevel.id, true,has ? 1 :0 ));
                //加入到一级树下面
            });
            firstLevel.ChildNodes.ForEach(x =>
            {
                x.showcheck = true;
                BuildTree(x, data, AuthIds);
            });
        }
        /// <summary>
        /// 创建树数据，根据传入的授权菜单的ID，来判断数据是否需要被勾上 （checkbox）
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="data"></param>
        /// <param name="authIds"></param>
        private static void BuildTree(MenuTreeModel parentNode, List<MenuConfigEntity> data, List<string> authIds)
        {
            var childList = data.Where(x => x.ParentId == parentNode.id).ToList();
            childList.ForEach(x =>
            {
                bool has = authIds.Any(p => p == x.ModuleId);
                parentNode.AddChild(new MenuTreeModel(x, parentNode.id, true, has ? 1 : 0));
                //加入到上一级级树下面
            });
            parentNode.ChildNodes.ForEach(x =>
            {
                x.showcheck = true;
                BuildTree(x, data, authIds);
            });
        }

        /// <summary>
        /// 创建树数据
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="data"></param>
        /// <param name="showCheckBox"></param>
        private static void BuildTree(MenuTreeModel parentNode, List<MenuConfigEntity> data, bool showCheckBox)
        {
            var childList = data.Where(x => x.ParentId == parentNode.id).ToList();
            childList.ForEach(x =>
            {
                parentNode.AddChild(new MenuTreeModel(x, parentNode.id, showCheckBox));
                    //加入到上一级级树下面
            });
            parentNode.ChildNodes.ForEach(x =>
            {
                BuildTree(x, data, showCheckBox);
            });
        }



        public static List<MenuTreeModel> BuildMenuTree(List<MenuConfigEntity> menus,bool showCheckBox = false)
        {
            var menuIds = menus.Select(x => x.ModuleId).ToList();
            var noParent = menus.Where(p => !menuIds.Contains(p.ParentId)).ToList();
                List<MenuTreeModel> list = new List<MenuTreeModel>();
                foreach (var firstLevel in noParent)
                {
                    MenuTreeModel tree = new MenuTreeModel(firstLevel,"0", showCheckBox);

                    if (menus.Any(p => p.ParentId == firstLevel.ModuleId))
                    {
                        BuildTree(tree, menus, showCheckBox);
                    }
                    list.Add(tree);
                }
            return list;

        }
        /// <summary>
        /// 移除没有授权的菜单，子集有数据不删除
        /// </summary>
        /// <param name="authTreeModel"></param>
        public static void RemoveNotAuthMenu(List<MenuTreeModel> authTreeModel)
        {
            authTreeModel.ForEach(tree => {
                tree.showcheck = false;
                bool anyCheck = false;
                if (tree.hasChildren)
                    DelChild(tree.ChildNodes, ref anyCheck );
            });
        }

        private static void DelChild(List<MenuTreeModel> childNodes,ref bool anyCheck)
        {
            var data = childNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                childNodes[i].showcheck = false;
                if (childNodes[i].checkstate == 1)//该子项被选中了
                {
                    //如果该项有子项，则继续循环找该项底下的子项
                    if (childNodes[i].hasChildren)
                    {
                        bool hasCheck = false;
                        DelChild(childNodes[i].ChildNodes, ref hasCheck);
                        if (!hasCheck)//子项没有一项被选中
                        {
                            //删除子项
                            childNodes[i].hasChildren = false;
                            childNodes[i].ChildNodes.Clear();
                        }
                    }
                    anyCheck = true;
                }
                else
                {
                    //该项没有被选中
                    //如果该项有子项，则继续循环找该项底下的子项
                    if (childNodes[i].hasChildren)
                    {
                        bool hasCheck = false;
                        DelChild(childNodes[i].ChildNodes, ref hasCheck);
                        if (!hasCheck)//子项没有一项被选中
                        {
                            //删除该项
                            childNodes.Remove(childNodes[i]);
                            anyCheck = false;
                        }
                        else
                        {
                            //子项有被选中,保留该项
                            anyCheck = true;
                        }
                    }
                    else
                    {
                        //该项没有子项
                        childNodes.Remove(childNodes[i]);//删除该项
                        anyCheck = false;
                    }
                }
            }
        }

        /// <summary>
        /// 查找某菜单下所有的下级
        /// </summary>
        /// <param name="menuConfigEntities">要过滤的所有的菜单（所有的菜单的集合）</param>
        /// <param name="childMenus">用来存储查找到的下级菜单的集合，不能为null</param>
        /// <param name="keyModuleId">要查找下级的菜单的Id</param>
        public static void FindAllChild(List<MenuConfigEntity> menuConfigEntities, List<MenuConfigEntity> childMenus, string keyModuleId)
        {
            var childrens = menuConfigEntities.Where(x => x.ParentId.Contains(keyModuleId)).ToList();
            childrens.ForEach(p =>
            {
                //继续查找下级
                FindAllChild(menuConfigEntities, childMenus, p.ModuleId);
                //将该项添加到子集里去
                childMenus.Add(p);
            });
        }
    }

}

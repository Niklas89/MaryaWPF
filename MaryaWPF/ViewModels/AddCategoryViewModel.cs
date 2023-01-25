using AutoMapper;
using Caliburn.Micro;
using LiveCharts.Wpf;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class AddCategoryViewModel : Screen
    {
        IServiceEndpoint _serviceEndpoint;
        IMapper _mapper;

        private CategoryDisplayModel _newCategory;
        public CategoryDisplayModel NewCategory
        {
            get { return _newCategory; }
            set
            {
                _newCategory = value;
                NotifyOfPropertyChange(() => NewCategory);
            }
        }

        private BindingList<CategoryDisplayModel> _categories;

        public BindingList<CategoryDisplayModel> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                NotifyOfPropertyChange(() => Categories);
            }
        }

        private string _categoryName;

        public string CategoryName
        {
            get { return _categoryName; }
            set
            {
                _categoryName = value;
                NotifyOfPropertyChange(() => CategoryName);
            }
        }

        public bool IsErrorVisible
        {
            get
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public AddCategoryViewModel(IServiceEndpoint serviceEndpoint, IMapper mapper)
        {
            _serviceEndpoint = serviceEndpoint;
            _mapper = mapper;
        }

        public void AddCategory(CategoryDisplayModel category, BindingList<CategoryDisplayModel> categories)
        {
            _newCategory = category;
            Categories = categories;
        }

        public async Task Add()
        {
            ErrorMessage = "";
            CategoryModel category = _mapper.Map<CategoryModel>(NewCategory);
            bool catAlreadyExist = false;

            try
            {
                // Remove all non letter characters
                CategoryName = new string((from c in CategoryName
                                           where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                  select c).ToArray());


                // Check if a category with the same name already exists
                foreach (var cat in Categories)
                {
                    if (CategoryName.Equals(cat.Name))
                    {
                        catAlreadyExist = true;
                        throw new Exception();
                    }
                }

                // Below lines are USEFUL for sending data to categoryEndPoint
                category.Name = CategoryName;

                // Below lines are USEFUL for INotifyPropertyChange 
                NewCategory.Name = CategoryName;

                // Add the new category
                await _serviceEndpoint.AddCategory(category);

                // After Add: get the last inserted category and insert it in the list bound to the datagrid
                await LoadCategoriesAfterAdd();

                Close();

            } catch(Exception ex)
            {
                if(catAlreadyExist) 
                    ErrorMessage = "Le nom de la catégorie existe déjà. Veuillez en choisir une autre.";
                else
                    ErrorMessage = ex.Message;
            }
            
        }

        // After Add: get the last inserted category and insert it in the list bound to the datagrid
        private async Task LoadCategoriesAfterAdd()
        {
            var categoriesListAfterAdd = await _serviceEndpoint.GetAll();
            var categoriesAfterAdd = _mapper.Map<List<CategoryDisplayModel>>(categoriesListAfterAdd);
            CategoryDisplayModel lastInsertedCategory = categoriesAfterAdd.LastOrDefault();
            Categories.Add(lastInsertedCategory);
        }

        public void Close()
        {
            TryCloseAsync();
        }

    }
}

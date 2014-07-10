using GenericServices.ServicesAsync;
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group10Actions
{
    class Test05ActionServiceEmptyActionAsync
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IActionServiceAsync<int, Tag> actionService = new ActionServiceAsync<int, Tag>(null, new EmptyTestActionAsync(false));
            IActionServiceAsync<int, Tag, SimpleTagDtoAsync> actionDtoService =
                new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(null, new EmptyTestActionAsync(false));

            //VERIFY
            (actionService is ActionServiceAsync<int, Tag>).ShouldEqual(true);
        }

        [Test]
        public async void Check02RunActionServiceSuccessOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(false);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = -456
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-456);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public async void Check03RunActionServiceCallSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag();
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(false);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database.");
        }

        [Test]
        public async void Check04RunActionServiceWarningsButSumbitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag(true)
            {
                TagId = 1 //means a warning
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database. (has 1 warnings)");
        }

        [Test]
        public async void Check05RunActionServiceWarningsNoSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag()
            {
                TagId = 1 //means a warning
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
            status.SuccessMessage.ShouldEndWith("... but NOT written to database as warnings. (has 1 warnings)");
        }

        [Test]
        public async void Check06RunActionServiceFailNoSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = 3 //will fail
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(false, status.Errors);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public async void Check10RunActionDtoSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(false);
            var service = new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDtoAsync
            {
                TagId = -123,
                Name = "test",
                Slug = "test"
            };
            var status = await service.DoActionAsync(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-123);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public async void Check11RunActionDtoSubmitOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDtoAsync
            {
                TagId = -123,
                Name = "test",
                Slug = "test"
            };
            var status = await service.DoActionAsync(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-123);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database.");
        }


        [Test] 
        public async void Check12RunActionDtoFailNoSubmitOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDtoAsync
            {
                TagId = 3,           //will fail
                Name = "test",
                Slug = "test"
            };
            var status = await service.DoActionAsync(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false, status.Errors);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }


    }
}

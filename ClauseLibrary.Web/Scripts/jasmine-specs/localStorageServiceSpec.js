describe('localStorageService', function () {

    var testScopeKey = 'localStorageTest';

    beforeEach(module('clauseLibraryApp'));
    afterEach(function () {
        // Remove all added entries in local storage.
        var prefix = testScopeKey + ".";
        var myLength = prefix.length;

        Object.keys(localStorage).forEach(function (key) {
            if (key.substring(0, myLength) === prefix) {
                window.localStorage.removeItem(key);
            }
        });
    });

    it('has root key defined for clause library', inject(function ($injector) {
        var service = $injector.get('localStorageService');

        expect(service.rootKey).toEqual('clauselibrary');
    }));
    
    it('can store and retrieve strings', inject(function ($injector) {
        var service = $injector.get('localStorageService');
        service.rootKey = testScopeKey;

        var key = "testItem";
        var value = "some string";
        expect(service.getItem(key)).toBeNull();

        service.setItem(key, value);
        expect(service.getItem(key)).toEqual(value);
    }));

    it('can transform the stored result when retrieving', inject(function ($injector) {
        var service = $injector.get('localStorageService');
        service.rootKey = testScopeKey;

        var key = "testItem";
        var value = "some string";
        expect(service.getItem(key)).toBeNull();

        service.setItem(key, value);
        expect(service.getItem(key, function(data) { return data.toUpperCase(); })).toEqual(value.toUpperCase());
    }));

    it('can remove strings', inject(function ($injector) {
        var service = $injector.get('localStorageService');
        service.rootKey = testScopeKey;

        var key = "testItem";
        var value = "some string";

        service.setItem(key, value);
        expect(service.getItem(key)).toEqual(value);

        service.removeItem(key);
        expect(service.getItem(key)).toBeNull();
    }));

    it('can store and retrieve objects', inject(function ($injector) {
        var service = $injector.get('localStorageService');
        service.rootKey = testScopeKey;

        var key = "testItem";
        var value = { name: "some object" };
        expect(service.getItem(key)).toBeNull();

        service.setItem(key, value);
        expect(service.getItem(key)).toEqual(value);
    }));

    it('can remove objects', inject(function ($injector) {
        var service = $injector.get('localStorageService');
        service.rootKey = testScopeKey;

        var key = "testItem";
        var value = { name: "some object" };

        service.setItem(key, value);
        expect(service.getItem(key, function (data) { return data || null; })).toEqual(value);

        service.removeItem(key);
        expect(service.getItem(key, function (data) { return data || null; })).toBeNull();
    }));

    it('can remove items where key starts with a defined string', inject(function ($injector) {
        var service = $injector.get('localStorageService');
        service.rootKey = testScopeKey;

        var key1 = "key1";
        var key2 = "key2";
        var value = { name: "some object" };

        service.setItem(key1, value);
        service.setItem(key2, value);
        expect(service.getItem(key1)).toEqual(value);
        expect(service.getItem(key2)).toEqual(value);

        service.removeItem(key1);
        expect(service.getItem(key1)).toBeNull();
        expect(service.getItem(key2)).toEqual(value);

    }));
});


lookup.platform_is_cordova = ko.observable(false);
lookup.platform_is_cordova_electron_mac = ko.observable(false);
lookup.platform_is_cordova_android = ko.observable(false);

lookup.check_platform = function()
{
    if(typeof(window.cordova) !== 'undefined')
    {
        lookup.platform_is_cordova(true);

        if(
            typeof(navigator) !== 'undefined' && 
            (navigator.platform === "MacIntel")
        )
        {
            lookup.platform_is_cordova_electron_mac(true);
        }
        else
        {
            lookup.platform_is_cordova_android(true);
        }
    }
    

    // if (window.cordova && cordova.platformId !== "browser") {
    //     lookup.platform_is_mobile(true);
    // }
};



lookup.privacy_policy_link = ko.computed(function()
{
    if(lookup.platform_is_cordova())
    {
        //&& window.cordova.platformId !== "electron"
        if(lookup.platform_is_cordova_electron_mac())
        {
            return "file:///Applications/WebPad.app/Contents/Resources/app.asar/privacy_policy_mac.html";
        }
        else
        {
            return "file:///android_asset/www/privacy_policy.html";
        }
        
    }
    else
    {
        return "/privacy_policy.html";
    }
});
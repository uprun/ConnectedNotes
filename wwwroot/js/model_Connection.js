function model_Connection(data)
{
    var id = data.id;
    var sourceId = data.sourceId;
    var destinationId = data.destinationId;
    var label = data.label;
    var generated = data.generated;
    var findNodeByIdFunc = data.findNodeByIdFunc;

    var self = this;
    self.id = id;
    self.SourceId = sourceId;
    self.DestinationId = destinationId;
    self.Source = findNodeByIdFunc(self.SourceId);
    self.Destination = findNodeByIdFunc(self.DestinationId);
    self.label = ko.observable(label);
    if(typeof(data.textChangedHandler) != "undefined")
    {
        self.label.subscribe(function(changes)
        {
            data.textChangedHandler(changes, self);
        });
    }
    self.labelAlmost = ko.computed(function()
    {
        var valueToCheck = self.label();
        if( 
            typeof(valueToCheck) == "undefined" ||
             valueToCheck == null || 
             valueToCheck == "")
             {
                 return "..";
             }
        else
        {
            return valueToCheck;
        }
    });
    self.labelUpdateCallback;
    self.generated = generated ? generated : false;
    self.underEdit = ko.observable(false);
    self.ConvertToJs = function() {
        return {
            id: self.id,
            SourceId: self.SourceId,
            DestinationId:  self.DestinationId,
            label: self.label(),
            generated: generated
        };
    };
};
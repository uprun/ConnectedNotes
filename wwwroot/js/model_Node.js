function model_Node(data)
{
    var self = this;
    self.id = data.id;
    self.text = ko.observable(data.text);
    self.x = data.x;
    self.y = data.y;
    self.ConvertToJs = function() {
        return {
            id: self.id,
            text: self.text(),
            x: self.x,
            y: self.y
        };
    };
};
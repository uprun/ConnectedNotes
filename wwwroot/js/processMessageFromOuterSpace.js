lookup.processMessageFromOuterSpace = function(item)
    {
        var current_action = item.action;
        var current_data = item.data;
        if(current_action == lookup.actions.NoteUpdated)
        {
            var found = lookup.findNodeById(current_data.id);
            if(found)
            {
                found.text(current_data.text);
                found.color = current_data.color;
                found.background = current_data.background;
                found.x = current_data.x;
                found.y = current_data.y;
            }
            else
            {
                var noteToAdd = new model_Node(current_data);
                lookup.Notes.push(noteToAdd);
            }
        }

        if(current_action == lookup.actions.ConnectionUpdated)
        {
            var found = lookup.findEdgeById(current_data.id);
            if(found)
            {
                found.label(current_data.label);
            }
            else
            {
                var connectionToAdd = 
                new model_Connection
                    (
                    current_data.id,
                    current_data.SourceId,
                    current_data.DestinationId,
                    current_data.label,
                    current_data.generated,
                    lookup.findNodeById
                    );
                lookup.Connections.push(connectionToAdd)
            }
        }

        if(current_action == lookup.actions.NoteAdded)
        {
            var found = lookup.findNodeById(current_data.id);
            if(!found)
            {
                var noteToAdd = new model_Node(current_data);
                lookup.Notes.push(noteToAdd);
            }
            else
            {
                if(!found.id.startsWith(lookup.localPrefix))
                {
                    found.text(current_data.text);
                }
            }
            
        }

        if(current_action == lookup.actions.NoteDeleted)
        {
            var found = lookup.findNodeById(current_data.id);
            if(found)
            {
                lookup.Notes.remove(found);
            }
        }

        if(current_action == lookup.actions.ConnectionAdded)
        {
            var found = lookup.findEdgeById(current_data.id);
            if(!found)
            {
                var connectionToAdd = 
                    new model_Connection
                    (
                        current_data.id,
                        current_data.SourceId,
                        current_data.DestinationId, 
                        current_data.label, 
                        current_data.generated,
                        lookup.findNodeById
                    );
                lookup.Connections.push(connectionToAdd)
            }
            else
            {
                if(!found.id.startsWith(lookup.localPrefix))
                {
                    found.label(current_data.label);
                }
            }
        }

        if(current_action == lookup.actions.ConnectionDeleted)
        {
            var found = lookup.findEdgeById(current_data.id);
            if(found)
            {
                lookup.Connections.remove(found);
            }
        }

        // if(current_action == lookup.actions.HealthCheckRequest)
        // {
        //     var toStoreNotes = ko.utils.arrayMap(lookup.Notes(), function(item) {
        //         return item.id;
        //     });
        //     var toStoreConnections = ko.utils.arrayMap(lookup.Connections(), function(item) {
        //         return item.id;
        //     });
        //     var all_available_ids = toStoreNotes.concat(toStoreConnections);
        //     var chunked_ids = all_available_ids.chunk(40);

        //     ko.utils.arrayForEach(chunked_ids, function(item, id) {
        //         self.pushToHistory({
        //             action: lookup.actions.HealthCheckIdsProposal,
        //             data: { 
        //                 checkedIndex: undefined,
        //                 publicKey: publicKey.publicKey 
        //             }
        //         });
        //     });


        // }
    };
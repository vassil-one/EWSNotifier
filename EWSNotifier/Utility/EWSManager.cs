using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EWSNotifier.ewswebreference;
using System.Net;
using EWSNotifier.Model;
using EWSNotifier.Utility;

namespace EWSNotifier
{
    /// <summary>
    /// Utility class for calling Exchange Web Service Operations
    /// </summary>
    public class EWSManager
    {
        private ExchangeServiceBinding EWSBinding { get; set; }

        public EWSManager(string username, string password, string domain, string ewsUrl)
        {
            EWSBinding = new ExchangeServiceBinding();
            EWSBinding.RequestServerVersionValue = new RequestServerVersion();
            EWSBinding.RequestServerVersionValue.Version = ExchangeVersionType.Exchange2007_SP1;
            EWSBinding.Credentials = new NetworkCredential(username, password, domain);
            EWSBinding.Url = ewsUrl;
        }

        public NTree<FolderType> FindFolders()
        {
            NTree<FolderType> treeRoot = new NTree<FolderType>(new FolderType());
            DistinguishedFolderIdType rootFolderId = new DistinguishedFolderIdType();
            rootFolderId.Id = DistinguishedFolderIdNameType.msgfolderroot;
            treeRoot = FindFolders(rootFolderId, treeRoot);
            return treeRoot;
        }

        public NTree<FolderType> FindFolders(BaseFolderIdType rootFolderId, NTree<FolderType> currentNode)
        {
            if (rootFolderId == null)
                return currentNode;

            // Create the request and specify the travesal type.
            FindFolderType findFolderRequest = new FindFolderType();
            findFolderRequest.Traversal = FolderQueryTraversalType.Shallow;

            // Define the properties that are returned in the response.
            FolderResponseShapeType responseShape = new FolderResponseShapeType();
            responseShape.BaseShape = DefaultShapeNamesType.Default;
            findFolderRequest.FolderShape = responseShape;

            // Add the folders to search to the request.
            findFolderRequest.ParentFolderIds = new BaseFolderIdType[] { rootFolderId };

            // Send the request and get the response.
            FindFolderResponseType findFolderResponse = EWSBinding.FindFolder(findFolderRequest);

            // Get the response messages.
            ResponseMessageType[] rmta = findFolderResponse.ResponseMessages.Items;
            ResponseMessageType rmt = rmta[0];

            // Cast to the correct response message type.
            FindFolderResponseMessageType ffrmt = (FindFolderResponseMessageType)rmt;

            // add each folder found to the current tree node, and recursively search each one
            var foldersFound = (from f in ffrmt.RootFolder.Folders where f.GetType() == typeof(FolderType) select f);
            for (int i = 0; i < foldersFound.Count(); i++)
            {
                FolderType f = (FolderType)foldersFound.ElementAt(i);
                currentNode.addChild(f);
                NTree<FolderType> newNode = currentNode.getChild(i);
                if (f.ChildFolderCount > 0)
                    FindFolders(f.FolderId, newNode);
            }

            return currentNode;
        }

        /// <summary>
        /// Subscribe to notifications for the given array of folders.
        /// </summary>
        /// <returns>SubscribeResponseMessageType which has the SubscriptionID and Watermark needed to GetEvents</returns>
        public SubscribeResponseMessageType Subscribe(BaseFolderType[] foldersToWatch)
        {
            PullSubscriptionRequestType request = new PullSubscriptionRequestType();
            request.FolderIds = (from f in foldersToWatch select f.FolderId).ToArray();
            request.EventTypes = new NotificationEventTypeType[] { NotificationEventTypeType.NewMailEvent };
            request.Timeout = 10; // ? 

            SubscribeType subscribe = new SubscribeType();
            subscribe.Item = request;

            SubscribeResponseType response = EWSBinding.Subscribe(subscribe);
            SubscribeResponseMessageType rmt = (SubscribeResponseMessageType)response.ResponseMessages.Items[0];

            return rmt;
        }

        /// <summary>
        /// Unsubscribe from the given subscription
        /// </summary>
        public void Unsubscribe(Subscription subscription)
        {
            UnsubscribeType unsubscribe = new UnsubscribeType();
            unsubscribe.SubscriptionId = subscription.SubscriptionId;
            EWSBinding.Unsubscribe(unsubscribe);
        }

        /// <summary>
        /// Call the GetEvents EWS operation for the given subscription
        /// </summary>
        public GetEventsResponseMessageType GetEvents(Subscription subscription)
        {
            GetEventsType get = new GetEventsType();
            get.SubscriptionId = subscription.SubscriptionId;
            get.Watermark = subscription.Watermark;
            GetEventsResponseType response = EWSBinding.GetEvents(get);
            GetEventsResponseMessageType rmt = (GetEventsResponseMessageType)response.ResponseMessages.Items[0];
            return rmt;
        }

        /// <summary>
        /// Call the Getitem EWS oepration for the given array of message item ids
        /// </summary>
        /// <returns>List of Messages</returns>
        public List<MessageType> GetMessages(BaseItemIdType[] itemIds)
        {
            List<MessageType> messages = new List<MessageType>();

            GetItemType get = new GetItemType();
            get.ItemShape = new ItemResponseShapeType() { BaseShape = DefaultShapeNamesType.AllProperties };
            get.ItemIds = itemIds;
            GetItemResponseType response = EWSBinding.GetItem(get);
            foreach (ItemInfoResponseMessageType iirmt in response.ResponseMessages.Items)
            {
                MessageType message = (MessageType)iirmt.Items.Items[0];
                messages.Add(message);
            }
            return messages;
        }


    }
}

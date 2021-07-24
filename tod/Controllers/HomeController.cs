using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using tod.Models;
using tod.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace tod.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly topxDbContext context;

        public HomeController(ILogger<HomeController> logger, topxDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Main() => View();

        [HttpGet]
        public IActionResult Newest()
        {
            List<Topic> newest = context.Topics.OrderByDescending(t => t.Id).Take(20).Include(t => t.User).ToList();
            return View(newest);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Recommended()
        {
            User user = GetUser();
            List<Tag> interests = user.Tags;
            List<Topic> alltopics = context.Topics.OrderByDescending(t => t.Id).Include(t => t.Tags).Include(t => t.User).ToList();

            List<Topic> topics = new();

            for (int i = 0; i < alltopics.Count(); i++)
                for (int j = 0; j < interests.Count(); j++)
                    if (alltopics[i].Tags.Contains(interests[j]) && !topics.Contains(alltopics[i]))
                        topics.Add(alltopics[i]);

            return View(topics);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Favorite()
        {

            User user = GetUser();
            List<Topic> favorite = context.Favorites.Where(t => t.User == user).Select(t => t.Topic).OrderByDescending(t => t.Id).ToList();
            foreach (var item in favorite)
            {
                item.User = context.Users.Where(u => u.Id == item.UserId).FirstOrDefault();
            }
            return View(favorite);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile(ProfileModel model)
        {
            model.User = GetUser();
            List<Tag> tags = model.User.Tags;
            string tagstexts = "";
            foreach (var item in tags)
            {
                tagstexts += item.Name + ", ";
            }
            model.TagsTexts = tagstexts;
            model.Tags = tags;
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            CreateModel model = new();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                List<Topic> topics = context.Topics.Where(t => t.Text == model.Text).Include(t => t.Tags).ToList();
                User user = GetUser();
                List<Tag> tags = new();
                if (!String.IsNullOrEmpty(model.TagsTexts))
                {
                    List<Tag> allTags = context.Tags.ToList();
                    List<string> tagsTexts = model.TagsTexts.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var item in tagsTexts)
                    {
                        Tag tag = new()
                        {
                            UserId = user.Id,
                            Name = item
                        };

                        Tag contextTag = allTags.Where(t => t.Name == tag.Name).FirstOrDefault();

                        if (contextTag == null)
                        {
                            context.Tags.Add(tag);
                            tags.Add(tag);
                        }
                        else tags.Add(contextTag);

                    }
                }

                if (topics.Count == 0)
                {
                    Topic topic = new()
                    {
                        User = user,
                        Text = model.Text,
                        Created = DateTime.Now,
                        Tags = tags
                    };

                    context.Topics.Add(topic);
                    context.SaveChanges();
                    return View("Main");
                }
                else
                {
                    ModelState.AddModelError("", "Such topic already exists - join it or create another one.");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyTopics()
        {
            User user = GetUser();
            List<Topic> topics = context.Topics.Where(t => t.User == user).OrderByDescending(t => t.Id).ToList();
            return View(topics);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Discussed()
        {
            User user = GetUser();

            List<Commentary> commentaries = context.Commentaries.Where(c => c.User == user).OrderByDescending(t => t.Id).Include(c => c.Topic).ToList();

            List<Topic> topics = new();
            foreach (var item in commentaries)
            {
                Topic topic = context.Topics.Where(t => t.Id == item.Topic.Id).Include(t => t.User).FirstOrDefault();
                if (!topics.Contains(topic))
                    topics.Add(topic);
            }

            return View(topics);
        }

        [HttpGet]
        public IActionResult Search(SearchViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public IActionResult Topic(int id)
        {

            Topic topic = context.Topics.Where(t => t.Id == id).Include(t => t.User).Include(t => t.Tags).Include(t => t.Reactions).Include(t => t.Commentaries).ThenInclude(c => c.Reactions).Include(t => t.Commentaries).ThenInclude(c => c.User).FirstOrDefault();

            TopicModel model = new()
            {
                Topic = topic,
                Id = topic.Id,
                TopicPositiveReactions = topic.Reactions.Where(r => r.Value == 1).Count(),
                TopicNegativeReactions = topic.Reactions.Where(r => r.Value == -1).Count()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Topic(TopicModel model)
        {
            Topic topic = context.Topics.Where(t => t.Id == model.Id).Include(t => t.User).Include(t => t.Commentaries).Include(t => t.Tags).Include(t => t.Reactions).FirstOrDefault();
            User user = GetUser();

            Commentary commentary = new()
            {
                User = user,
                Text = model.CommentText,
                Topic = topic,
                Created = DateTime.Now
            };

            List<Commentary> commentaries = context.Commentaries.Where(t => t.Topic.Id == topic.Id).Include(c => c.User).ToList();

            if (!String.IsNullOrEmpty(model.CommentText))
            {
                commentaries.Add(commentary);

                topic.Commentaries = commentaries;

                context.Topics.Update(topic);
                context.SaveChangesAsync();
            }
            else
            {
                topic.Commentaries = commentaries;
            }


            model.Topic = topic;
            model.CommentText = "";

            return RedirectToAction("Topic", model);

        }

        [Authorize]
        public IActionResult AddToFavorite(int TopicId, string User)
        {

            User user = context.Users.Where(u => u.Nickname == User).Include(u => u.Tags).FirstOrDefault();
            Topic topic = context.Topics.Where(t => t.Id == TopicId).Include(t => t.User).Include(t => t.Tags).FirstOrDefault();
            Topic favorite = context.Favorites.Where(t => t.Topic.Id == TopicId && t.User == user).Select(f => f.Topic).FirstOrDefault();
            if (favorite == null)
            {
                context.Favorites.Add(new Favorite() { User = user, Topic = topic });

                TopicModel model = new()
                {
                    Topic = topic
                };

                topic.User.Rating += 1.0;

                context.SaveChanges();
            }

            return RedirectToAction("Topic", new { id = TopicId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangeInterests(ProfileModel model)
        {
            User user = GetUser();
            List<Tag> oldTags = user.Tags;

            List<string> tagsTexts = model.TagsTexts.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<Tag> tags = new();
            foreach (var item in tagsTexts)
            {
                Tag tag = new()
                {
                    //User = user,
                    UserId = user.Id,
                    Name = item
                };

                Tag userTag = oldTags.Where(t => t.Name == tag.Name).FirstOrDefault();
                Tag contextTag = context.Tags.Where(t => t.Name == tag.Name).FirstOrDefault();

                if (userTag == null && contextTag == null)
                {
                    tags.Add(tag);
                    context.Tags.Add(tag);
                }
                if (userTag == null && contextTag != null)
                {
                    tags.Add(contextTag);
                }
                if (userTag != null && userTag != null)
                {
                    tags.Add(userTag);
                }
            }

            user.Tags = tags;

            context.Users.Update(user);

            context.SaveChangesAsync();

            return RedirectToAction("Profile", model);
        }

        [Authorize]
        public IActionResult TopicReact(int value, int TopicId)
        {

            User user = GetUser();
            Topic topic = context.Topics.Where(t => t.Id == TopicId).Include(t => t.User).Include(t => t.Reactions).ThenInclude(r => r.ReactedUser).FirstOrDefault();
            TopicReaction TopicReaction = topic.Reactions.Where(r => r.ReactedUser == user).FirstOrDefault();

            if (TopicReaction == null)
            {
                TopicReaction reaction = new()
                {
                    ReactedUser = user,
                    Value = value,
                };

                if (topic.User != user)
                    topic.User.Rating += value;

                topic.Reactions.Add(reaction);
                context.TopicReactions.Add(reaction);
                context.SaveChanges();
            }
            if (TopicReaction != null && TopicReaction.Value != value)
            {
                TopicReaction reaction = new()
                {
                    ReactedUser = user,
                    Value = value
                };

                if (topic.User != user)
                    topic.User.Rating += value * 0.75;

                topic.Reactions.Remove(TopicReaction);
                topic.Reactions.Add(reaction);
                context.TopicReactions.Remove(TopicReaction);
                context.TopicReactions.Add(reaction);
                context.SaveChanges();
            }

            return RedirectToAction("Topic", new { id = TopicId });
        }

        [Authorize]
        public IActionResult CommentReact(int value, int CommentId, int TopicId)
        {
            User user = GetUser();
            Commentary commentary = context.Commentaries.Where(c => c.Id == CommentId).Include(c => c.Reactions).Include(c => c.User).FirstOrDefault();
            CommentaryReaction commentaryReaction = commentary.Reactions.Where(r => r.ReactedUser == user).FirstOrDefault();

            if (commentaryReaction == null)
            {
                CommentaryReaction reaction = new()
                {
                    ReactedUser = user,
                    Value = value
                };

                if (commentary.User != user)
                    commentary.User.Rating += value * 0.25;

                commentary.Reactions.Add(reaction);
                context.CommentaryReactions.Add(reaction);
                context.SaveChanges();
            }
            if (commentaryReaction != null && commentaryReaction.Value != value)
            {
                commentaryReaction.Value = value;

                if (commentary.User != user)
                    commentary.User.Rating += value * 0.25;

                context.CommentaryReactions.Update(commentaryReaction);
                context.SaveChanges();
            }

            return RedirectToAction("Topic", new { id = TopicId });
        }

        [HttpPost]
        public IActionResult Find(SearchViewModel model)
        {
            if (!String.IsNullOrEmpty(model.Author) || !String.IsNullOrEmpty(model.Title) || !String.IsNullOrEmpty(model.Tags))
            {
                List<Topic> topics = context.Topics.Include(t => t.User).Include(t => t.Tags).ToList();


                if (!String.IsNullOrEmpty(model.Author))
                    topics = topics.Where(t => t.User.Nickname == model.Author).ToList();

                if (!String.IsNullOrEmpty(model.Title))
                {
                    topics = topics.Where(t => t.Text.Contains(model.Title, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }

                if (!String.IsNullOrEmpty(model.Tags))
                {
                    List<string> tagsKeywords = model.Tags.Split(new string[] { " ", ",", ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<Tag> tags = new();
                    List<Topic> topicsByTags = new();

                    foreach (var item in tagsKeywords)
                        tags.AddRange(context.Tags.Where(t => t.Name == item));

                    foreach (var item in tags)
                        foreach (var topicItem in topics)
                        {
                            if (topicItem.Tags.Contains(item))
                                if (!topicsByTags.Contains(topicItem))
                                    topicsByTags.Add(topicItem);
                        }

                    List<Topic> result = new();
                    result.AddRange(topics);

                    if (topics.Count != 0)
                        foreach (var item in topics)
                            if (!topicsByTags.Contains(item))
                                result.Remove(item);

                    topics = result;
                }

                model.Topics = topics;
            }

            return View("Search", model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult TopicEditing(int Id)
        {
            Topic topic = context.Topics.Where(t => t.Id == Id).Include(t => t.Tags).Include(t => t.Reactions).Include(t => t.Commentaries).FirstOrDefault();

            CreateModel model = new()
            {
                Topic = topic,
                Text = topic.Text,
                Id = topic.Id
            };

            foreach (var item in topic.Tags)
                model.TagsTexts += item.Name + ", ";

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult TopicEditing(CreateModel model)
        {
            List<Tag> tags = new();
            User user = GetUser();

            Topic topic = context.Topics.Where(t => t.Id == model.Id).Include(t => t.Tags).Include(t => t.Reactions).Include(t => t.Commentaries).FirstOrDefault();

            if (!String.IsNullOrEmpty(model.TagsTexts))
            {
                List<Tag> allTags = context.Tags.ToList();
                List<string> tagsTexts = model.TagsTexts.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var item in tagsTexts)
                {
                    Tag tag = new()
                    {
                        UserId = user.Id,
                        Name = item
                    };

                    Tag contextTag = allTags.Where(t => t.Name == tag.Name).FirstOrDefault();

                    if (contextTag == null)
                    {
                        context.Tags.Add(tag);
                        tags.Add(tag);
                    }
                    else tags.Add(contextTag);

                }
            }

            topic.Text = model.Text;
            topic.Tags = tags;

            context.Update(topic);
            context.SaveChanges();
            return RedirectToAction("Topic", new { topic.Id });
        }

        [Authorize]
        public IActionResult DeleteTopic(int Id)
        {
            Topic topic = context.Topics.Where(t => t.Id == Id).Include(t => t.User).Include(t => t.Tags).Include(t => t.Reactions).Include(t => t.Commentaries).FirstOrDefault();

            if (User.Identity.Name == topic.User.Nickname)
            {
                context.Remove(topic);
                context.SaveChanges();
            }

            return RedirectToAction("Newest");
        }

        [Authorize]
        [HttpPost]
        public IActionResult UserEditing(ProfileModel model)
        {
            if (ModelState.IsValid)
            {
                User user = GetUser();
                User nameUser = context.Users.Where(u => u.Nickname == model.NewName).FirstOrDefault();
                User emailUser = context.Users.Where(u => u.Email == model.NewEmail).FirstOrDefault();

                model.User = user;
                List<Tag> tags = model.User.Tags;
                string tagstexts = "";
                foreach (var item in tags)
                {
                    tagstexts += item.Name + ", ";
                }
                model.TagsTexts = tagstexts;
                model.Tags = tags;

                if (nameUser != null && emailUser != null)
                {
                    ModelState.AddModelError("NewName", "User with such nickname already exists.");
                    ModelState.AddModelError("NewEmail", "User with such email already exists");
                    return View("Profile", model);
                }
                if (nameUser != null && emailUser == null)
                {
                    ModelState.AddModelError("NewName", "User with such nickname already exists.");
                    model.User = user;
                    return View("Profile", model);
                }
                if (nameUser == null && emailUser != null)
                {
                    ModelState.AddModelError("NewEmail", "User with such email already exists");
                    model.User = user;
                    return View("Profile", model);
                }

                if (!String.IsNullOrEmpty(model.NewName))
                    if (nameUser == null && emailUser == null)
                        user.Nickname = model.NewName;

                if (!String.IsNullOrEmpty(model.NewEmail))
                    if (nameUser == null && emailUser == null)
                        user.Email = model.NewEmail;

                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                AuthorizationController controller = new(context);
                _ = controller.Authenticate(model.NewName);


                context.Update(user);
                context.SaveChanges();
            }

            return RedirectToAction("Profile", model);
        }

        private User GetUser()
        {
            User user = context.Users.Where(u => u.Nickname == User.Identity.Name).Include(u => u.Tags).FirstOrDefault();
            return user;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}